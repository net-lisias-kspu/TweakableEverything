#!/bin/bash

d=`pwd`
cd GameData
/usr/bin/find . -iname \*.dll -exec rm {} \;
cd $d
for i in *; do
	if [ "$i" != "GameData" -a -d $i ]; then
		cd $i
		/usr/bin/find bin -name ${i}.dll -print -exec cp {} ${d}/GameData/TweakableEverything/PlugIns \; 
		
		cd $d
	fi
done

cd ${d}/../ToadicusTools
/usr/bin/find bin -name ToadicusTools.dll -print -exec cp {} ${d}/GameData/ToadicusTools/PlugIns \; 
cp ToadicusTools.version  ${d}/GameData/ToadicusTools
cp /d/Users/jbb/github/MiniAVC.dll ${d}/GameData/ToadicusTools
cp /d/Users/jbb/github/MiniAVC.dll ${d}/GameData/TweakableEverything

cd $d  

v="TweakableEverything.version"
cp $v ${d}/GameData/TweakableEverything
major=`grep -m 1 MAJOR TweakableEverything.version  | cut -f2 -d':' | tr -d , | tr -d '[:space:]'`
minor=`grep -m 1 MINOR TweakableEverything.version  | cut -f2 -d':' | tr -d ,| tr -d '[:space:]'`
patch=`grep -m 1 PATCH TweakableEverything.version  | cut -f2 -d':' | tr -d ,| tr -d '[:space:]'`
version="${major}.${minor}.${patch}"

releasedir=/d/Users/jbb/release
fname="${releasedir}/TweakableEverything-${version}.zip"
zip -9r $fname GameData
