@echo off
if not exist "packages\fake" goto :download

SetLocal EnableDelayedExpansion

if /i !1!=="-forceupdate" (
    shift
    goto :download
)

goto run

:download
build\nuget install fake -outputdirectory packages -excludeversion

:run
packages\fake\tools\fake .\build\build.fsx %1 %2 %3 %4 %5 %6 %7 %8

::    Copyright (c) Attila Kisk�, enyim.com
::
::    Licensed under the Apache License, Version 2.0 (the "License");
::    you may not use this file except in compliance with the License.
::    You may obtain a copy of the License at
::
::        http://www.apache.org/licenses/LICENSE-2.0
::
::    Unless required by applicable law or agreed to in writing, software
::    distributed under the License is distributed on an "AS IS" BASIS,
::    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
::    See the License for the specific language governing permissions and
::    limitations under the License.
::
