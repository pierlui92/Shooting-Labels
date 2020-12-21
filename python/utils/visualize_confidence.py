from utils import *
import argparse
from open3d import *

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, help="input folder with ply")
parser.add_argument("--classes", type=str, default='../unity/Assets/CSV/ColorConfiguration.csv', help="classes csv")
parser.add_argument("--cmap", type=str, default='magma', help="confidence cmap")
parser.add_argument("--threshold", type=float, default=0, help="confidence threshold")
parser.add_argument("--save", action='store_true', help="save visualization")
parser.add_argument("--output_path", type=str, help="output")
args=parser.parse_args()

id2color,color2id,id2color_lut,names,num_classes = decode_classes(args.classes)
vertices, faces, ids, colors, confidence = read_ply_with_custom_fields(args.input_path, read_confidence=True)
colors = colors.astype(float)

if args.threshold > 0:
    sem_color_face=[id2color_lut[i] for i in ids]
    colors[faces]=np.stack([sem_color_face,sem_color_face,sem_color_face],axis=1) 
    colors = colors / 255
    colors[faces[confidence <= args.threshold]] = np.asarray([1,0,0])
else:
    import matplotlib.cm as cm
    from matplotlib.colors import Normalize
    mycm = cm.get_cmap(args.cmap)
    norm = Normalize(vmin=0,vmax=1)
    uncert = 1-confidence
    conf_cm = mycm(norm(uncert))[:,:-1]
    colors[faces]=np.stack([conf_cm,conf_cm,conf_cm],axis=1)

mesh = TriangleMesh()
mesh.vertices= Vector3dVector(vertices)
mesh.vertex_colors= Vector3dVector(colors)
mesh.triangles= Vector3iVector(faces)

if args.save:
    write_triangle_mesh(args.output_path, mesh)

draw_geometries([mesh])

