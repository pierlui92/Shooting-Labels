import os
import argparse

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, default='pcd.ply', help="input ply")
parser.add_argument("--output_path", type=str, default='../splits/', help="output ply")
parser.add_argument("--id_mapping", type=str, default='utils/mapping_nyu402eigen13_v2.txt', help="mapping from id to id")
args=parser.parse_args()

lines=open(args.id_mapping).readlines()
id2id={}
for l in lines:
    l=l.strip()
    id2id[int(l.split(",")[0])]=int(l.split(",")[1])

f=open(args.input_path)
fout=open(args.output_path,"w")
for l in f.readlines():
    x,y,z,id = l.strip().split(" ")
    new_label=id2id[int(id)]
    fout.write(x + " " + y + " " + z + " " + str(new_label) + "\n" )

f.close()
fout.close()