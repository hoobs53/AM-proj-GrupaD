#/usr/bin/python3
import cgi
import cgitb;
import json
from sense_hat import SenseHat
sense = SenseHat()
cgitb.enable()
print("Content-Type: text/html")
print()
form = cgi.FieldStorage()
print(form["LED00"].value)
id = "LED"
for i in range(8):
    for j in range(8):
        id_temp = id+str(j)+str(i)
        if id_temp in form:
            obj = form[id_temp].value
            tab = json.loads(obj)
            x = int(tab[0])
            y = int(tab[1])
            r = int(tab[2])
            g = int(tab[3])
            b = int(tab[4])
            rgb_valid = r>=0 and r<=255 and g>=0 and g<=255 and b>=0 and b<=255
            if rgb_valid:
                sense.set_pixel(x, y, r, g, b)
            else:
                print("Error in rgb_valid")
