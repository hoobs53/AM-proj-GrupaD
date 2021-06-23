#!/usr/bin/python3

import random
import cgi
import cgitb; cgitb.enable()
import json
import time
from sense_hat import SenseHat

sense = SenseHat()
try:
	while True:
		result = {}
		orientation = sense.get_orientation_degrees()
		result["roll"] = orientation["roll"]
		result["pitch"] = orientation["pitch"]
		result["yaw"] = orientation["yaw"]
		
		jsonResult = json.dumps(result)
		with open("/home/pi/server/rpy.json", "w") as f:
			f.write(jsonResult)
		time.sleep(0.05)
except KeyboardInterrupt:
	pass
