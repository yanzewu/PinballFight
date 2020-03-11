import cv2
import numpy as np
import sys

if __name__ == '__main__':

	src = cv2.imread(sys.argv[1], cv2.IMREAD_UNCHANGED)

	src[(src[:,:,0] == 255)*(src[:,:,1] == 255)*(src[:,:,2] == 255),3] = 0;

	cv2.imwrite(sys.argv[1], src)
