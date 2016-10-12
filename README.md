# graf3d

graf3d is simple 3D graphics rendering engine written in C#.
It implements:
* Line clipping algorithms (Liang-Barsky).
* Line rasterization algorithms (Bresenham, Recursive Midpoint).
* Specular reflection algorithms (Phong, Blinn-Phong).
* Bump mapping using normal maps.
* Translations, scaling and perspective.
* Rotations with quaternions.

Sample scene is imported from Unity exported JSON file.

# Projects structure
* `Engine/` - class library with no external dependencies implementing all graphics rendering.
* `Demo/` - WPF application rendering sample scene.

# Demos

### Zooming
![Zoom](Screenshots/zoom.gif)

### Camera movement
![Movement](Screenshots/movement.gif)

### Specular relflection
![Reflection](Screenshots/reflection.gif)

### Bump mapping
![Bump mapping](Screenshots/bumpmapping.gif)

### Normal map used for bump mapping
![Rock normal map](Demo/Resources/rock.png)
