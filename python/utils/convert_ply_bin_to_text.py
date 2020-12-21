from open3d import *
import argparse

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, default='pcd.ply', help="input ply")
parser.add_argument("--output_path", type=str, default='pcd_text.ply', help="output ply")
args=parser.parse_args()

pcd = read_point_cloud(args.input_path)
points = np.asarray(pcd.points)
colors = np.asarray(pcd.colors)

f=open(args.output_path, "w")
f.write("ply\n")
f.write("format ascii 1.0\n")
f.write("element vertex %d\n" % points.shape[0])
f.write("property float x\n")
f.write("property float y\n")
f.write("property float z\n")
f.write("property uchar red\n")
f.write("property uchar green\n")
f.write("property uchar blue\n")
f.write("end_header\n")

for i in range(points.shape[0]):
    f.write(str(points[i][0]) + " " + str(points[i][1]) + " " + str(points[i][2]) + " " + str(colors[i][0]) + " " + str(colors[i][1]) + " " + str(colors[i][2]) +"\n" )

f.close()
