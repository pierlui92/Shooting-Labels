#!/bin/bash
for f in *; do mv $f "${f%\(Clone\).ply}.ply"  ;done