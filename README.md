#__Brain Strain__

##About
Brain Strain is 3D multiplatform puzzle game with cubes and numbers made in Unity.
The game is avalable in two languages - English and Bulgarian and in two platforms - Windows and Android.

##Rules
1. Break apart the object into sets of blocks connected by their faces(sides)
2. Each set must include the number of blocks shown on the numbered blocks
3. Each set must have exacly one block with a diggit on it

##Download
You can download the most recen version of the game builded in the bin forlder depending on your platform:
###[Windows][]
###[Android][]

##TODO
[High priority]
- Add block type to ID [0%] <-
- Clen up dirty code mess
  - Clean up Block class hierarchy [90%] <-
- Add sounds]
  - Fix Android bug when loading settings file <-
  - Create Audio class for playing everything(don't use Global) [90%]
  - Make the mute buttons available in-game and redesign the Audio settings GUI [20%]
  - Main soundtrack (temp placeholder) [90%]
  - Hit block sfx (temp placeholder) [90%]
  - Undo block sfx [0%]
  - Level complete sfx [0%]
  - others that I can't think of right now :D
[Llow priority]
- Make random level generation [0%]
- Add Hilbert curve cube level [0%]
- Make level creator [0%]
- Add more blocks
  - 'portal' blocks [0%]
  - TNT block [0%]
  - others [0%]
- Maybe add some encryption to save files (but later when everything is stable) [0%]

*full TODO [here][todo]*

##License
  ```
  Copyright 2013 Borislav Kosharov

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
  ```
  
[windows]: https://github.com/nikibobi/BrainStrain/tree/master/bin/Windows
[android]: https://github.com/nikibobi/BrainStrain/tree/master/bin/Android
[todo]: https://github.com/nikibobi/BrainStrain/blob/master/src/Assets/TODO.txt
