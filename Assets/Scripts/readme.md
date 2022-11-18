Scripts for the car configurator.

The intended structure is:

 - The `Root` behaviour belongs to the parent of all cars.
 - Each car belongs to an object with a `Car` behaviour.
 - The `Car` may have a `Configurator` on the same GameObject, or one or more `Configurator` children, which represent submenus containing different variant options.
 - Children which represent parts or materials that can be swapped out should have one of the three behaviours which implement the `IPart` interface. These can be a `CustomisablePart`, a `CustomisablePaint` or a `ToggleablePart`.
 - If a customisable part consists of multiple GameObjects, only one should implement `CustomisablePart`. The rest should have `DerivedPart` behaviours. In the current design, the user is responsible for making sure that the derived part has the same variants in the same order as the main `CustomisablePart`---this design could be improved to avoid this possible source of bugs.
 - The camera should have a `CameraController`.

The `UI` folder contains UXML templates for the menus.