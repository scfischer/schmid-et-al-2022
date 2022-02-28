# _Alvin_ 
### Interactive, Visual Simulation of a Spatio-Temporal Model of Gas Exchange in the Human Alveolus.

This is the source code of the _Alvin_ simulation software version 1.1, published in: 

Schmid, Kerstin, Andreas Knote, Alexander Mück, Keram Pfeiffer, Sebastian von Mammen, 
and Sabine C. Fischer. 2022. ‘Interactive, Visual Simulation of a Spatio-Temporal Model of 
Gas Exchange in the Human Alveolus’. _Frontiers in Bioinformatics_ 1.

You can download the application from the binaries folder or from http://go.uniue.de/alvin.
If you use _Alvin_ or the source code, please cite the publication above.


| Project | _Alvin_ |
| ----- | -------- |
| Engine (version) | Unity 2020.1.16f1 |
| Contributors | Alexander Mück, Andreas Knote, Kerstin Schmid |
| Contacts | Kerstin Schmid, kerstin.schmid@uni-wuerzburg.de & Sabine Fischer, sabine.fischer@uni-wuerzburg.de |

### Repository Organization

```
RepositoryRoot/
    ├── binaries             // Archives (.zip) of builds for Windows, macOS and Linux
    ├── code 		     // The project source code
    ├── documentation        // Doxygen manual 
    ├── publication
    │   ├── poster           // PDF of the presentation poster
    │   └── paper            // PDF of the publication 
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