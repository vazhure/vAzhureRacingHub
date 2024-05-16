# rFactorSharedMemoryMap

A plugin for rFactor 1-based sims to export the standard telemetry and scoring data structs to a shared memory mapped file handle.
This allows external programs to access the sim data without affecting frame times in the simulator.

Details of the shared memory map can be found in `Include\rfSharedStruct.hpp`. The plugin is based on the sample plugin code from ISI found at http://rfactor.net/web/rf1/devcorner/ and compiled using Visual Studio Community 2015.

A sample application using Python to access the memory map can be found in https://github.com/dallongo/pySRD9c.

### Releases
#### 2016-05-11 (v2.0.0.0)

* Added Vehicle Info interpolation between Scoring updates
* Changed meaning of deltaTime to be time since last Scoring update
* Changed inRealtime to be set/unset by Enter/ExitRealtime methods
* Added clean up before and after each session

#### 2016-05-09 (v1.1.0.0)

* Updated `rfVehicleInfo` to include acceleration and rotation vectors

#### 2016-05-08 (v1.0.0.0)

* Initial release
