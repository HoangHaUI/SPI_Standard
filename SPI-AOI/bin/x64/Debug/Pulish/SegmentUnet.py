import cv2
import numpy as np
import datetime
import time
import Utils


def SegmentImage(model, image, image_width, image_height, debug):
    if(debug):
        st = time.time()
    mask = np.zeros(image.shape[:2], np.uint8)
    imgs, locs = Utils.Split_Image(image, None, image_width, image_height)
    if debug:
        print("[INFO] | {} | Split {} images...".format(datetime.datetime.now(),len(imgs)))
    data_input = Utils.Convert_Image(imgs)
    results = model.predict(data_input, batch_size = 10)
    for i, imgp in enumerate(results):
        img_predict = np.argmax(imgp, axis=2)
        img_predict[img_predict == 1] = 255
        img_predict = img_predict.astype(np.uint8)
        x, y, w, h = locs[i]
        mask[y:y+h, x:x+w] = img_predict
    if(debug):
        print("[INFO] | {} | Predict in {} seconds...".format(datetime.datetime.now(),time.time() - st))
    return mask

