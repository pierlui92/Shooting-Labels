import os
import numpy as np
from plyfile import PlyData, PlyElement

def read_ply_with_custom_fields(path, read_ids=True, read_colors=True, read_confidence=False):
    plydata = PlyData.read(path)
    x=np.asarray(plydata['vertex']['x'])
    y=np.asarray(plydata['vertex']['y'])
    z=np.asarray(plydata['vertex']['z'])
    vertices=np.stack([x,y,z],axis=-1)
    faces=np.vstack(plydata['face']['vertex_indices'])
    
    if read_ids:
        ids=np.asarray(plydata['face']['category_id'])
    else:
        ids = np.zeros_like(faces)
    
    if read_colors:
        r=np.asarray(plydata['vertex']['red'])
        g=np.asarray(plydata['vertex']['green'])
        b=np.asarray(plydata['vertex']['blue'])
        colors=np.stack([r,g,b],axis=-1)
    else:
        colors=np.zeros_like(vertices)
    
    if read_confidence:
        confidence=np.asarray(plydata['face']['confidence'])
        return vertices, faces, ids, colors, confidence

    return vertices, faces, ids, colors

def write_ply_with_custom_fields(path_output,vertices,colors,faces,ids, confidence=None, multiplier_rgb=255):
    f=open(path_output, "w")
    f.write("ply\n")
    f.write("format ascii 1.0\n")
    f.write("element vertex %d\n" % len(vertices))
    f.write("property float x\n")
    f.write("property float y\n")
    f.write("property float z\n")
    f.write("property uchar red\n")
    f.write("property uchar green\n")
    f.write("property uchar blue\n")
    f.write("element face %d\n" % len(faces))
    f.write("property list uchar int vertex_indices\n")
    f.write("property int category_id\n")
    if confidence:
        f.write("property float confidence\n")
    f.write("end_header\n")
    #Write all vertices with color
    for j in range(0,len(vertices)):
        coord = np.asarray(vertices[j])
        f.write('%f %f %f ' % (coord[0],coord[1],coord[2]))
        red = int(colors[j][0]*multiplier_rgb)
        green = int(colors[j][1]*multiplier_rgb)
        blue = int(colors[j][2]*multiplier_rgb)
        f.write('%d %d %d\n' % (red,green,blue))
    for j in range(0,len(faces)):
        if confidence:    
            f.write("3 %d %d %d %d %f\n" % (faces[j][0],faces[j][1],faces[j][2],ids[j],confidence[j]))
        else:
            f.write("3 %d %d %d %d\n" % (faces[j][0],faces[j][1],faces[j][2],ids[j]))
    f.close()

def decode_classes(path_classes, max_range_rgb=255):
    names=[]
    id2color={}
    id2color_lut=[[0,0,0]]*256
    color2id={}
    cl=open(path_classes).readlines()[1:]
    num_classes=len(cl)

    for l in cl:
        id,name,r,g,b=l.strip().split(",")
        names.append(name)
        id=int(id)
        color2id[r+" "+g+" "+b]=id
        r=int(r)
        g=int(g)
        b=int(b)
        id2color[id] = np.asarray([r,g,b])
        id2color_lut[id] = np.asarray([r,g,b])

    return id2color, color2id, id2color_lut, names, num_classes

def decode_categories(path_cat,num_classes):
    cat=open(path_cat).readlines()[1:]
    cat2id_lut=[0]*(len(cat) + 1)
    for l in cat:
        idx,raw_cat,cat,count,nyuId,nyu40id,eigen13id,nyuClass,nyu40class,eigen13class,ModelNet40,ModelNet10,ShapeNetCore55,synsetoffset,wnsynsetid,wnsynsetkey,mpcat40index,mpcat40=l.strip().split("\t")
        if num_classes >= 40:
            if nyu40id != "":
                cat2id_lut[int(idx)] = int(nyu40id)
        else:
            if eigen13id != "":
                cat2id_lut[int(idx)] = int(eigen13id)
    return cat2id_lut