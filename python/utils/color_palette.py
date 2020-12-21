from __future__ import division
import argparse
from matplotlib import pyplot as plt
from matplotlib import colors as mcolors

parser = argparse.ArgumentParser()
parser.add_argument("--classes", type=str, default='../../unity/Assets/CSV/ColorConfiguration.csv', help="classes csv")
args=parser.parse_args()

cl=open(args.classes).readlines()[1:]
names=[]
colors_classes=[]
colors={}
for l in cl:
    id,name,r,g,b=l.strip().split(",")
    names.append(name)
    r=float(r)/255
    g=float(g)/255
    b=float(b)/255
    colors_classes.append((r,g,b))
    colors[name] = (r,g,b)

# Sort colors by hue, saturation, value and name.
by_hsv = sorted((tuple(mcolors.rgb_to_hsv(mcolors.to_rgba(color)[:3])), name)
                for name, color in zip(names,colors_classes))

sorted_names = [name for hsv, name in by_hsv]

n = len(sorted_names)
ncols = 4
nrows = n // ncols + 1

fig, ax = plt.subplots(figsize=(8, 5))

# Get height and width
X, Y = fig.get_dpi() * fig.get_size_inches()
h = Y / (nrows + 1)
w = X / ncols

for i, name in enumerate(sorted_names):
    col = i % ncols
    row = i // ncols
    y = Y - (row * h) - h

    xi_line = w * (col + 0.05)
    xf_line = w * (col + 0.25)
    xi_text = w * (col + 0.3)

    ax.text(xi_text, y, name, fontsize=(h * 0.8),
            horizontalalignment='left',
            verticalalignment='center')

    ax.hlines(y + h * 0.1, xi_line, xf_line,
              color=colors[name], linewidth=(h * 0.6))

ax.set_xlim(0, X)
ax.set_ylim(0, Y)
ax.set_axis_off()

fig.subplots_adjust(left=0, right=1,
                    top=1, bottom=0,
                    hspace=0, wspace=0)
plt.show()
