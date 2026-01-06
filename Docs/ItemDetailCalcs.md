# Goal
- The thought is to have a border area between the inset where the actual item image appears and the surrounding bounds for the entire item detail canvas; this border area can be filled with the image of e.g. a picture frame to make a nice border graphic.
- We want our border images to stretch/compress as necessary to fill a prefabricated shape outline determined by relative aspect ratios. This way we can swap item border frame images without resizing them manually and everything should work on any screen resolution.
- Ideally we'd specify that the border image has a cut-out in its shape to accomodate the item detail image inset shape, but most engines won't support comlpex geometry like that for the UI. Dunno if Unity would, but it's computationally simpler to just have a transparency in the border image for the cut-out section and treat the entire border image as an overlay layer.
- The key point of interest is making sure the transparent area of the border image matches the aspect ratio of the item detail image inset shape. I'm pretty sure there's no way around manual image transforms at the asset level to achieve this since it will be a potentially different operation per border image.

# Using Component Aspect Ratios Relative to Total Screen Dimensions
- I'm not sure these are super useful since the images and canvasi will be added as transform children of the constraining parents, so as far as each child is concerned the constraining parent dimensions are the total surrounding dimensions.
- On the other hand, our purpose for these calcs is to use in modding the asset files directly, so the transform hierarchy is not relevant. It would probably be much less confusing to use standard ratio numbers rather than percentages of unknown dimensions, even though I think the latter will still work.

## Total screen dimensions
### Reference Resolution
- 800px x 600px
### Raw Measure on 3440px x 1440px Display
- 30cm x 17cm

## Room View aspect ratio (room_ar)
- 76% width x 66% height
- 2:1
### Raw Measure
- 19cm x 9.5cm

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
### Raw Measure
- ~12cm x ~5.5cm
### Total Screen Percentage Measure
- 12.3cm x 5.4cm
- So that method pretty much works out. Not exactly straightforward, though, so I'm going to skip ahead to using the actual standalone aspect ratio of the detail image inset bounds, which is all we need. 

# Using Item Detail Inset Aspect Ratio Directly
## Inset Aspect Ratio (inset_ar)
- ~~2.2:1~~
- 1.5:1
## Detail Image Aspect Ratio (image_ar)
- 1.5:1
- Probably the inset_ar should match image_ar; not sure why I didn't design it that way originally.
   - I want to achieve 1.5:1 for the image_ar within a X:Y for the item_ar within a 2:1 for room_ar.
   - First step is to find what the item_ar X:Y aspect ratio will be if the image_ar is 1.5:1.
      - We define our image_ar in terms of percentage of item_ar, as .8item_ar.x : .8item_ar.y, but since we're talking about aspect ratio and the percentages are the same for each dimen we'll want item_ar to also be 1.5:1
   - Second step is to figure out how we achieve 1.5:1 for item_ar expressed as percentage of room_ar's 2:1?
         - item_ar.x = .75room_ar.x
         - item_ar.y = room_ar.y
         - item_ar.x_min = 12.5%
         - item_ar.x_max = 87.5%
         - item_ar.y_min = 0%
         - item_ar.y_min = 100%
         - So to keep that aspect ratio but cover different actual screen area, we can take percentages of the above percentages and as long as we apply the same percentage to all dimensions we should keep our aspect ratio. I'd like the width coverage to be about 50%, so we'll go from there:
            - 0.75X = 0.5, X = 0.667 or ~66%
            - item_ar.x = .5room_ar.x
            - item_ar.y = .66room_ar.y
            - item_ar.x_min = 25%
            - item_ar.x_max = 75%
            - item_ar.y_min = 16.5%
            - item_ar.y_min = 83.5%
   - Third step is adjusting the above so that we have a constant depth (I guess you'd call it?) of border frame. This is tricky because a depth of Ncm is going to be a different percentage of the item_ar.x than item_ar.y since the aspect ratio is not 1:1. In order to build in extra room for a frame around the inset_ar that we need to be 1.5:1, we'll need to modify item_ar.
      - I think it would be easiest to have two guiding principles:
         1. A given Ncm frame depth we want all around the inset bounds.
         1. Our set 1.5:1 inset_ar. 
      - I like the look of 0.5cm frame depth on my reference display, which is 5% of above item_ar.x.
      - So from 0.5cm frame depth and a 1.5:1 inset_ar, we need to figure out what our item_ar and item view percentages become and then modify our inset view percentages to work out to a 1.5:1 inset_ar.
      - Hmm, I think this might be impossible the way I'm trying to do it i.e. with both frame and detail inset bounds given as percentages of the item display bounds.
         - What we could do instead, if we must avoid any content wrapping and keep constraints in terms of percentages of parents, is have a second canvas for the frame which will be installed centered over (or under) the item detail canvas (origin at same coords). Then as long as the frame's cut-out transparency matches the inset_ar, we should get a scenario where we effectively slot the item detail canvas into the cut-out... except matching inset_ar isn't going to be sufficient. We'd need the cut-out dimens to actually match those of the detail inset, else the aspect ratio doesn't help. To get that, we'd need to scale the entire frame image by an arbitrary amount, however much is required to make the cut-out match when taking the arbitrary dimens of the frame graphic elements themselves into account.
         - So basically metadata about the frame size and how much it adds around the cut-out so we can calculate the appropriate scale factor? Or I guess the actual pixel dimens of the cut-out would do since the frame elements have to run along its edges and its what we really care about anyway. Might be able to get all that data at runtime from the engine.
         - Else, could just say f***it.js to the algebra and layering and have the four frame elements as separate assets that we bolt on around the edges of our detail inset panel, which I guess in this scenario will become the entire item detail canvas.

# Frame Image Modification
- Once we have the item_ar and inset_ar above, we'll need to edit any given frame image so that the total aspect ratio matches item_ar and the transparent rectangle cut-out matches inset_ar. That way we can have the picture frame image as a background and overlay the item detail image in the inset panel, and the detail image should slot right in nicely to the cut-out. 