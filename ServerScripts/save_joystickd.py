#!/usr/bin/python3
import sys
import time
import json
from sense_hat import SenseHat, ACTION_RELEASED
sense = SenseHat()
try:
	with open ("joystick.json") as f:
		data = json.load(f)
		x = data['x']
		y = data['y']
		mid_counter = data['mid_counter']
except:
	x = 0
    y = 0
    mid_counter = 0

def up(event):
    global y
    if event.action != ACTION_RELEASED:
        y = y+1
def down(event):
    global y
    if event.action != ACTION_RELEASED:
        y =  y-1
def left(event):
    global x
    if event.action != ACTION_RELEASED:
        x =  x-1
def right(event):
    global x
    if event.action != ACTION_RELEASED:
        x =  x+1
def middle(event):
    global mid_counter
    if event.action != ACTION_RELEASED:
        mid_counter =  mid_counter+1
    
sense.stick.direction_up = up
sense.stick.direction_down = down
sense.stick.direction_left = left
sense.stick.direction_right = right
sense.stick.direction_middle = middle

try:
	while True:
		dict = {"x": x, "y": y, "mid_counter": mid_counter}
		result = json.dumps(dict)
		with open("joystick.json", "w") as f:
			f.write(result)
		time.sleep(0.05)
except KeyboardInterrupt:
	pass
