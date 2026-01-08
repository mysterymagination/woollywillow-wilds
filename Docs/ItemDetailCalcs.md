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
#### GardenCanvas Dimensions
- 1337.5px x 600px

## Room View aspect ratio (room_ar)
- 75% width x 66.66% height
- 1003.125px x 400.02px
- ~2.51:1
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
   - Second step is to figure out how we achieve 1.5:1 for item_ar expressed as percentage of room_ar's ~~2:1~~ 2.5:1?
         - item_ar.x = .6room_ar.x
         - item_ar.y = room_ar.y
         - item_ar.x_min = 20%
         - item_ar.x_max = 80%
         - item_ar.y_min = 0%
         - item_ar.y_min = 100%
         - So to keep that aspect ratio but cover different actual screen area, we can take percentages of the above percentages and as long as we apply the same percentage to all dimensions we should keep our aspect ratio. I'd like the width coverage to be about 50%, so we'll go from there:
            - 0.6X = 0.5, X = 0.833 or ~83% to go from 60% to 50% on X, so we need to apply ~83% to Y as well.
            - item_ar.x = .5room_ar.x
            - item_ar.y = .83room_ar.y
            - item_ar.x_min = 25%
            - item_ar.x_max = 75%
            - item_ar.y_min = 8.5%
            - item_ar.y_min = 91.5%
         - But then there's not much room for the surrounding frame, so :
            - 0.6X = 0.5, X = 0.833 or ~83% to go from 60% to 50% on X, so we need to apply ~83% to Y as well.
            - item_ar.x = .5room_ar.x
            - item_ar.y = .83room_ar.y
            - item_ar.x_min = 25%
            - item_ar.x_max = 75%
            - item_ar.y_min = 8.5%
            - item_ar.y_min = 91.5%
   - Third step is adjusting the above so that we have a constant depth (I guess you'd call it?) of border frame. This is tricky because a depth of Ncm is going to be a different percentage of the item_ar.x than item_ar.y since the aspect ratio is not 1:1. In order to build in extra room for a frame around the inset_ar that we need to be 1.5:1, we'll need to modify item_ar.
      - I think it would be easiest to have two guiding principles:
         1. A given Ncm frame depth we want all around the inset bounds.
         1. Our set 1.5:1 inset_ar. 
      - I like the look of 0.5cm frame depth on my reference display, which is 5% of above item_ar.x.
      - So from 0.5cm frame depth and a 1.5:1 inset_ar, we need to figure out what our item_ar and item view percentages become and then modify our inset view percentages to work out to a 1.5:1 inset_ar.
      - Hmm, I think this might be impossible the way I'm trying to do it i.e. with both frame and detail inset bounds given as percentages of the item display bounds.
         - What we could do instead, if we must avoid any content wrapping and keep constraints in terms of percentages of parents, is have a second canvas for the frame which will be installed centered over (or under) the item detail canvas (origin at same coords). Then as long as the frame's cut-out transparency matches the inset_ar, we should get a scenario where we effectively slot the item detail canvas into the cut-out... except matching inset_ar isn't going to be sufficient. We'd need the cut-out dimens to actually match those of the detail inset, else the aspect ratio doesn't help. To get that, we'd need to scale the entire frame image by an arbitrary amount, however much is required to make the cut-out match when taking the arbitrary dimens of the frame graphic elements themselves into account.
         - So basically metadata about the frame size and how much it adds around the cut-out so we can calculate the appropriate scale factor? Or I guess the actual pixel dimens of the cut-out would do since the frame elements have to run along its edges and its what we really care about anyway. Might be able to get all that data at runtime from the engine.
         - Else, could just say f***it.js to the algebra and layering and have the four frame elements as separate assets that we bolt on around the edges of our detail inset panel, which I guess in this scenario will become the entire item detail canvas. Could even do a cheeky coupla panels surrounding the item canvas with a constrained size at build time so there's no need to adjust anything at runtime; our frame components can just be loaded into those panels and they'll scale to fit automatically.
      - Waitaminute, hang on, I actually pulled out some graph paper and it seems like it should be possible after all. How about a 5:4 or 1.25:1 item_ar and then inset_ar.x can be .6item_ar.x and inset_ar.y can be .5item_ar.y? That should give us 1.5:1 for the inset and the remaining surround should be a uniform depth/width/whatever border?
         1. First step is getting the constraint percentages of room_ar that will give us a 1.25:1 item_ar:
            1. room_ar is ~2.5:1, so to get to 1.25 we need 0.5item_ar.x and let item_ar.y fill room_ar.y
            1. item_ar.x_min = 0.25
            1. item_ar.x_max = 0.75
            1. item_ar.y_min = 0
            1. item_ar.y_max = 1
            1. To leave some room on Y, we'll scale both dimens down by TODO
         1. Second step is defining our 1.5:1 inset_ar as percentages of the parent item_ar.
            1. Expanding my 1.25:1 to 5:4 for simplicity, we can get to a child 3:2 (reduceable to 1.5:1) by taking 60% of item_ar.x and 50% of item_ar.y
            1. inset_ar.x_min = 0.2
            1. inset_ar.x_max = 0.8
            1. inset_ar.y_min = 0.25
            1. insert_ar.y_max = 0.75
         - This works! Gives us a constant border of about 2.5cm on my reference screen. However! If I scale my 1.5:1 inset_ar dimens to try to leave less of a gigantic border area, the proportions are no longer equal all around. I'm not quite sure on the math behind this, but it seems we need very specific relative dimens to make this effect work. Curious! Worth a deeper look at some point.  
            - You can also get passably close by making item_ar 1.5:1 via 50% X and 83% Y of the 2.5:1 room_ar and then inset just a constant percentage of parent dimens so it will alsob e 1.5:1
            - Anyway, this whole approach of trying to match a frame image transparent cut-out over our item image inset introduces headaches with the actual surrounding frame size since that will also be bounded and will contribute to the overall frame image dimens. We can prep the asset with a 1.5:1 cut-out and then scale the loaded frame image at runtime by whatever it takes to match the runtime measured dimens of the item detail canvas, but then the system will perform further scaling out of our control to make the arbitrary frame surround dimens fit with the bounds; this will mess up the alignment of the cut-out. 


# Frame Image Modification
- Once we have the item_ar and inset_ar above, we'll need to edit any given frame image so that the total aspect ratio matches item_ar and the transparent rectangle cut-out matches inset_ar. That way we can have the picture frame image as a background and overlay the item detail image in the inset panel, and the detail image should slot right in nicely to the cut-out. 

# Bolt-on Additive Frame Strat
- Notion is to make our 1.5:1 item inset and leave room surrounding it to add on panels that can host the left, top, right, bottom edges of the frame surround elements.
- This means we don't need to modify the assets (necessarily) except to make a separate image per frame surround element.
- todo: This almost worked, except I see an odd misalignment of the textures after constraint scaling occurs in the surround panels, even after accounting for the 2.5:1 (dimens uniformly scaled down from room_ar) item_ar.
   - I think this may have occurred because the original image aspect ratio is vastly different from the 2.5:1 we're trying to shove it into?