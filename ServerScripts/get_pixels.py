#!/usr/bin/python3

import cgi
import cgitb; cgitb.enable()
import json
from sense_hat import SenseHat

print("Content-type: application/json\n")

sense = SenseHat()

print(json.dumps(sense.get_pixels()))
