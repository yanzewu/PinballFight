
1. Avoid directly using Unity's asset packages (which can be downloaded from store), especially linking them by drag-and-drop.
External asset packages will dramatically increase the size of repository. If you have to use it, move the component you use (usually images) to Resouces/ or Images/, and put the rest of package into Exclude/

2. If you are >90% sure that the image (or anything else) will not be frequently modified (especially renamed), put it into Images/. Otherwise put it into Resources/Images/, where everything is loaded dynamically.

