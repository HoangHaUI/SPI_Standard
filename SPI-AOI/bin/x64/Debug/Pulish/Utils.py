import base64
import numpy as np

def GetBase64File(path):
    bytedata = b''
    with open(path, "rb") as f:
        byte = f.read(64)
        while byte != b'':
            # Do stuff with byte.
            bytedata += byte
            byte = f.read(64)
    base64EncodedStr = base64.b64encode(bytedata)
    return base64EncodedStr

def Image2Str(img_path):
    base64EncodedStr = GetBase64File(img_path)
    return base64EncodedStr.decode()


def Convert_Image(imgarray):
    x = np.array(imgarray)
    x = x.astype(np.float32) / 255.0
    x = np.expand_dims(x, axis=-1)
    return x

def ReConvert_Image(imgarray):
    x = np.argmax(imgarray, axis=3)
    x[x == 1] = 255
    x[x == 2] = 255
    x = x.astype(np.uint8)
    x = np.reshape(x, (x.shape[0], x.shape[1], x.shape[2]))
    return x




def Split_Image(img, ROI, width_target, height_target):
    if ROI is not None:
        x, y, w, h = ROI
    else:
        x, y, w, h = 0, 0, img.shape[1], img.shape[0]

    img_roi  = img[y: y + h, x:x + w]
    #shape element
    he, we = width_target, height_target
    st_x_dirs = w // we if w % we == 0 else w // we + 1
    st_y_dirs = h // he if h % he == 0 else h // he + 1
    img_split = []
    locs = []
    for stx in range(st_x_dirs):
        for sty in range(st_y_dirs):
            xt = stx * we
            yt = sty * he
            xt = xt  if xt + we <= w - 1 else w - 1 - we
            yt = yt if yt + he <= h - 1 else h - 1 - he
            loc = (xt + x, yt + y, we, he)
            imge = img_roi[yt:yt+he, xt:xt+we]
            img_split.append(imge)
            locs.append(loc)
    
    img_split  = np.array(img_split)
    return img_split, locs


# def Split_Image(img, ROI, width_target, height_target, step_ratio = 0.25):
#     if ROI is not None:
#         x, y, w, h = ROI
#     else:
#         x, y, w, h = 0, 0, img.shape[1], img.shape[0]
#     img_roi  = img[y: y + h, x:x + w]
#     #shape element
#     we, he = width_target, height_target
#     pbackx = int(step_ratio * width_target)
#     pbacky = int(step_ratio * height_target)
#     img_split = []
#     locs = []
#     xt = - pbackx
#     yt = - pbacky
#     breakx = False
#     breaky = False
#     while 1:
#         breaky = False
#         yt = - pbacky
#         xt += pbackx
#         if xt + we > w -1:
#             xt = w - 1 - we
#             breakx = True
#         while 1:
#             yt += pbacky
#             if yt + he > h - 1:
#                 yt = h - 1 - he
#                 breaky = True
#             loc = (xt + x, yt + y, we, he)
#             imge = img_roi[yt:yt+he, xt:xt+we]
#             img_split.append(imge)
#             locs.append(loc)
#             if breaky:
#                 break
#         if breakx:
#             break
#     img_split  = np.array(img_split)
#     return img_split, locs