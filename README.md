# _Alvin_ 
### Interactive, Visual Simulation of a Spatio-Temporal Model of Gas Exchange in the Human Alveolus.

This is the source code of the _Alvin_ simulation software version 1.1, published in: 

Schmid K, Knote A, Mück A, Pfeiffer K, Mammen S von, Fischer SC. 2021. Interactive, Visual Simulation of a 
Spatio-Temporal Model of Gas Exchange in the Human Alveolus. doi: 10.1101/2021.09.15.460416.

The latest version of _Alvin_ is always available at http://go.uniwue.de/alvin.
If you use _Alvin_ or the source code, please cite the publication above.


| Project | _Alvin_ |
| ----- | -------- |
| Engine (version) | Unity 2020.1.16f1 |
| Contributors | Alexander Mück, Andreas Knote, Kerstin Schmid |
| Contacts | Kerstin Schmid, kerstin.schmid@uni-wuerzburg.de & Sabine Fischer, sabine.fischer@uni-wuerzburg.de |

### Repository Organization

```
RepositoryRoot/
    ├── binaries             // Archives (.zip) of the most recent build
    ├── code 		     // The project source code
    ├── documentation        // Doxygen manual 
    ├── publication
    │   ├── poster           // PDF of the presentation poster
    │   └── paper            // PDF of the preprint 
    ├── LICENSE              // License terms
    ├── NOTICES              // Attributions  
    ├── README.md            // This file
    └── REFERENCES           // References cited in the documentation 
```
This repository uses [Git LFS](https://git-lfs.github.com/), which means that files are replaced by text pointers inside Git, 
while the file contents are stored on a remote server like GitHub.com or GitHub Enterprise. To access the files in this repository,
you will need to clone it.

### Special Mentions:
This project was originally developed as project work in the course "Interactive Visualization and Simulation in Biology and Life Sciences" @JMU Würzburg, 
summer semester 2020. Thanks to Sebastian v. Mammen, Sabine Fischer and Andreas Knote and all other participants for helping to make it happen.

Additional thanks go to Keram Pfeiffer, who was willing to test _Alvin_ in his physiology lab course and provided valuable suggestions for improving and 
adding functionality to the application.