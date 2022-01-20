# Reinforced-Concrete-Membrane

This library is part of the research published in [Structures Journal](https://authors.elsevier.com/a/1e1Wj8MoIGt99M).

Library for nonlinear analysis of reinforced concrete membrane elements by [**Modified Compression Field Theory** (VECCHIO; COLLINS, 1986)](https://doi.org/10.14359/12115), [**Disturbed Stress Field Model** (VECCHIO, 2000)](https://doi.org/10.1061/(ASCE)0733-9445(2000)126:9(1070)) and [**Softened Membrane Model** (HSU; ZHU, 2002)](https://doi.org/10.14359/12115).

This library uses:

- [MathNet.Numerics](https://github.com/mathnet/mathnet-numerics) for Linear Algebra operations;

- [Units.NET](https://github.com/angularsen/UnitsNet) for simple unit conversions;

- [Material](https://github.com/andrefmello91/Material) for concrete and reinforcement constitutive models.

- [OnPlaneComponents](https://github.com/andrefmello91/On-Plane-Components) for stress and strain transformations;

- [Extensions](https://github.com/andrefmello91/Extensions) for some numeric extensions.

## Class Membrane

The membrane object can be initiated by static typing:

`var membrane = Membrane.From([Concrete Parameters], [Web Reinforcement], [Width], [Constitutive Model])`

## Class MembraneSolver

The MembraneSolver class implements a nonlinear solution algorithm for solving a membrane element with known applied stresses.

Example:

`var solver = new MembraneSolver([Membrane Element], [Nonlinear Solution Method])`

`solver.Solve([Applied Stresses], [Simulate until failure?])`

`solver.OutputResults(out [File Path], [Location to write file])`

A csv file will be saved in the output location.

## Usage

### Package reference:

`<PackageReference Include="andrefmello91.ReinforcedConcreteMembrane" Version="1.X.X" />`

### .NET CLI:

`dotnet add package andrefmello91.ReinforcedConcreteMembrane --version 1.X.X`