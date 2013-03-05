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
- Clen up dirty code mess
  - GUI classes need lots of cleanup [50%]
  - Make Block class hierarchy instead of using one class(this will make adding new blocks a lot easyer) [0%]
- Make the Parser to be able to serialize(save) a level in a file [0%]
- Add sounds [0%]
- Add Hilbert curve cube level [0%]
- Make random level generation [0%]
- Make level creator [0%]
- Add more blocks
  - 'portal' blocks [0%]
  - TNT block [0%]
  - others [0%]

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
