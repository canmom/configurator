Scripts for the car configurator.

The intended structure is:

 - The `Root` behaviour belongs to the parent of all cars.
 - Each car belongs to an object with a `Configurator` behaviour.
 - Children which represent parts or materials that can be swapped out should have one of the three behaviours which implement the `IPart` interface. These can be a `CustomisablePart`, a `CustomisablePaint` or a `ToggleablePart`.
 - The camera should have a `CameraController`.

The `UI` folder contains UXML templates for the menus.