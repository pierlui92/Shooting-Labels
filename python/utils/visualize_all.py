import os
from open3d import *

a =[]

path="F:\\matterport_v2\\integration\\"

for f in os.listdir(path):
    a.append(read_triangle_mesh(os.path.join(path,f)))

draw_geometries(a)