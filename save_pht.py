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
		humidity = sense.get_humidity()
        pressure = sense.get_pressure()
        temperature = sense.get_temperature()
		result["humidity"] = humidity
		result["pressure"] = pressure
		result["temperature"] = temperature
		
		jsonResult = json.dumps(result)
		with open("/home/pi/server/pht.json", "w") as f:
			f.write(jsonResult)
		time.sleep(0.05)
except KeyboardInterrupt:
	pass
