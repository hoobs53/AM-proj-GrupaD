#!/usr/bin/python3

import random
import cgi
import cgitb; cgitb.enable()
import json
from sense_hat import SenseHat

print("Content-type: application/json\n")

sense = SenseHat()
result = {}
orientation = sense.get_orientation_degrees()
result["roll"] = orientation["roll"]
result["pitch"] = orientation["pitch"]
result["yaw"] = orientation["yaw"]
print(json.dumps(result))