# Goal
- The thought is to have a border area between the inset where the actual item image appears and the surrounding bounds for the entire item detail canvas; this border area can be filled with the image of e.g. a picture frame to make a nice border graphic.
- We want our border images to stretch/compress as necessary to fill a prefabricated shape outline determined by relative aspect ratios. This way we can swap item border frame images without resizing them manually and everything should work on any screen resolution.
- Ideally we'd specify that the border image has a cut-out in its shape to accomodate the item detail image inset shape, but most engines won't support comlpex geometry like that for the UI. Dunno if Unity would, but it's computationally simpler to just have a transparency in the border image for the cut-out section and treat the entire border image as an overlay layer.
- The key point of interest is making sure the transparent area of the border image matches the aspect ratio of the item detail image inset shape. I'm pretty sure there's no way around manual image transforms at the asset level to achieve this since it will be a potentially different operation per border image.

# Component Aspect Ratios Relative to Total Screen Dimensions
- I'm not sure these are super useful since the images and canvasi will be added as transform children of the constraining parents, so as far as each child is concerned the constraining parent dimensions are the total surrounding dimensions.
- On the other hand, our purpose for these calcs is to use in modding the asset files directly, so the transform hierarchy is not relevant. It would probably be much less confusing to use standard ratio numbers rather than percentages of unknown dimensions, even though I think the latter will still work.

## Room View aspect ratio (room_ar)
- 76% width x 66% height

## Item Detail View outer aspect ratio (item_ar)
- (.8room_ar.x - .2room_ar.x) x (.8room_ar.y - .2room_ar.y)
   - 0.608 - 0.152 = 0.456
   - 0.528 - 0.132 = 0.396
   - 0.456 width x 0.396 height

## Item Detail View inset aspect ratio (inset_ar)
- (.95item_ar.x - .05item_ar.x) x (.9item_ar.y - .1item_ar.y)
   - 0.4332 - 0.0228 = 0.4104
   - 0.3564 - 0.0396 = 0.3168
   - 0.4104 width x 0.3168 height