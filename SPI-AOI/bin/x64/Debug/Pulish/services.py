import numpy as np
import cv2
import os
from flask import Flask, flash, request, redirect, url_for
import time
import datetime
import base64
from pyzbar.pyzbar import decode, bounding_box
from keras.models import load_model
import tensorflow as tf
import Config
import SegmentUnet
import Utils
import Decoder

app = Flask(__name__)
# app.config["DEBUG"] = True
physical_devices = tf.config.experimental.list_physical_devices("GPU")
if(len(physical_devices) > 0):
    tf.config.experimental.set_memory_growth(physical_devices[0], True)
SEGMENT_MODEL = load_model(Config.SEGMENT_MODEL_PATH)
SEGMENT_MODEL.summary()
SegmentUnet.ActiveGPU(SEGMENT_MODEL, Config.IMAGE_WIDTH, Config.IMAGE_HEIGHT)


@app.route('/', methods=['GET', 'POST'])
def Action():
    global SEGMENT_MODEL
    if request.method == 'POST':
        result  = {}
        if(request.values.get("Type") == "Test"):
            result["status"] = "OK"
            return str(result)
        if 'file' not in request.files:
            flash('No file part')
            result["status"] = "FAIL"
            print("[INFO] | {} |{}".format(datetime.datetime.now(), "No file part"))
            return str(result)
        file = request.files.getlist("file")
        for f in file:
            if f.filename == '':
                result["status"] = "FAIL"
                print("[INFO] | {} |{}".format(datetime.datetime.now(), "Not found file"))
                return str(result)
            filestr = f.read()
            npimg = np.fromstring(filestr, np.uint8)
            action_type = request.values.get("Type")
            img = cv2.imdecode(npimg, 1)

            # segment
            if(action_type == "Segment"):
                img = cv2.medianBlur(img, 3)
                stt = request.values.get("FOV")
                debug = request.values.get("Debug") == "True"
                mask = SegmentUnet.SegmentImage(SEGMENT_MODEL, img, Config.IMAGE_WIDTH, Config.IMAGE_HEIGHT, debug)
                name = "Result/mark" + str(stt) + ".png"
                cv2.imwrite(name, mask)
                imageStr = Utils.Image2Str(name)
                result["status"] = "OK"
                result["image"] = imageStr
                return str(result)

            # decode
            elif action_type == "Decode":
                img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
                sn = Decoder.Decode(img_gray)
                result["status"] = "OK"
                result["sn"] = sn
                print(str(result))
                return str(result)

    elif request.method == 'GET':
        return "SPI segment service..."
    

    
if __name__ == "__main__":
    app.run(host="0.0.0.0", port=812)
