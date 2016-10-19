#!/bin/bash

d=`pwd`
cd GameData
/usr/bin/find . -iname \*.dll -exec rm {} \;
cd $d
for i in *; do
	if [ -d $i ]; then
		cd $i
		/usr/bin/find bin -name ${i}.dll -print -exec cp {} ${d}/GameData/TweakableEverything \; 
		cd $d
	fi
done
