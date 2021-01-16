import cv2
import numpy as np 
from pyzbar.pyzbar import decode, bounding_box


def Decode(img):
    sn = "NOT FOUND"
    rect = (0,0,0,0)
    try:
        rows,cols = img.shape
        result_data = []
        for n in range(1, 5 ,1):
            if(len(result_data) > 0):
                break
            if( n > 1):
                resized_img= cv2.resize(img,(n*rows,n*cols))
            else:
                resized_img = img
            for al_val in range(0,25):
                alpha = 1 + al_val*0.1
                result_img = cv2.convertScaleAbs(resized_img, alpha=alpha, beta=0)
                result_data = decode(result_img)
                if(len(result_data) > 0):
                    for code in result_data:
                        sn = code.data.decode("utf-8")
                    break
        return sn
    except Exception as err:
        print(err)
        return sn, rect


if __name__ == "__main__":
    img = cv2.imread("4.png", 0)
    sn = Decode(img)
    print(sn)