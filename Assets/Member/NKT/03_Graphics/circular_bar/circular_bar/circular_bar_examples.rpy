################################################################################
##
## Circular Bar for Ren'Py by Feniks (feniksdev.itch.io / feniksdev.com)
##
################################################################################
## This file contains an example screen demonstrating how to use the circular
## bar screen language statement. You are free to delete this file if you don't
## need the bar examples; all the backend code is in 01_circular_bar.rpy.
##
## To see the example circular bar screen, include a button to see it from the
## main menu e.g.
##
# textbutton "Circular Bar Examples" action ShowMenu("circular_bar_screen")
##
## Leave a comment on the tool page on itch.io if you run into any issues.
################################################################################
################################################################################
## BAR PROPERTIES
##
## Circular bars have several properties that regular bars do not, and deal with
## some regular bar properties in different ways. These are:
##
# circumference : int
#     The number of degrees the bar should take up. A number between 1
#     and 360. Defaults to 360. Note that, even for bars with less than 360
#     degrees, *your bar images must still have the center of the bar at the
#     center of the image*. So, if your image is a half-circle from 12:00 to
#     6:00, the half of the image from 6:00 to 12:00 must *still be present* as
#     transparent space or similar. Bar images are expected to be square for
#     this reason, though transparency and focus_mask can be used to make them
#     appear otherwise. This property MUST be declared on the screen.
# start_angle : int
#     The number of degrees relative to 12:00 the bar begins at. Must
#     be between -360 and 360. Defaults to 0, which means the bar begins
#     at 12:00. MUST be declared on the screen.
# thumb_rotate : bool
#     Whether or not the thumb should rotate with the bar. Defaults to
#     False. If the thumb isn't rotating with the bar, you should
#     typically provide a thumb_offset. MUST be declared on the screen.
# thumb_offset : int
#     The distance from the center of the bar to the center of the thumb.
#     Defaults to 0.
# start_thumb : Displayable or True
#     A displayable that's permanently displayed at the start of the bar.
#     Also takes prefixes like hover_ or selected_. If it is True, it will be
#     identical to thumb (and its prefixed versions). MUST be declared on the
##    screen.
# hide_start_thumb : bool
#     A boolean that hides the start_thumb when the bar is full or empty.
#     Defaults to False. MUST be declared on the screen.
# overshoot : float
#     The percent of the bar that acts as a buffer zone when going from
#     0 to 100% or vice versa. Defaults to 0.25. This means that if you
#     fill up the bar, you can drag up to 125% of the bar and it will
#     remain at 100% until you lift your mouse and start dragging again.
#     The same happens for dragging past 0%. A value of 0 means there is
#     no buffer and the bar will jump from 0 to 100% and vice versa.
#     MUST be declared on the screen.
# focus_mask : displayable, True, callable, or None
#     A mask that's used to control what portions of the button can be
#     focused, and hence clicked on. The type of this property determines
#     how it is interpreted. See:
#     https://www.renpy.org/doc/html/style_properties.html#style-property-focus_mask
# bar_invert : bool
#     If True, the bar will fill up counter-clockwise instead of clockwise.
#     Defaults to False.
# fore_bar : Displayable
#     A displayable that's used for the bar before it's full (i.e. when it's
#     empty). Think of it as fore = beFORE it is full.
# aft_bar : Displayable
#     A displayable that's used for the bar after it's full. Think of it as
#     AFTer it is full.
# fore_gutter : int
#     The number of degrees reserved for the "gutter" before the bar fills. So,
#     if you had a bar which spanned 180 degrees and the fore_gutter was 10, you
#     could drag it from 10 to 180 degrees and the first 10 degrees would be
#     reserved. Defaults to 0.
# aft_gutter : int
#     The number of degrees reserved for the "gutter" after the bar fills. So,
#     if you had a bar which spanned 180 degrees and the aft_gutter was 10, you
#     could drag it from 0 to 170 degrees and the last 10 degrees would be
#     reserved. Defaults to 0.
##
## If a property is noted as *MUST be declared on the screen*, that means it
## has to be provided on the screen itself when you declare the circular_bar.
## Otherwise, it can be included in an external style.
## Besides the above mentioned properties, all other bar properties can be
## used for circular bars as well, such as thumb, base_bar, thumb_shadow, mouse,
## unscrollable, and keyboard_focus, as well as actions like hovered, released,
## and changed.
################################################################################
screen circular_bar_screen():

    tag menu
    add "#282634"

    style_prefix 'rbar'
    default bar_page = 0
    default max_bar_pages = 4

    default animated_adjustment = AnimatedValue(60, 60, 12, 0)
    default percentage_value = 65
    default pie_value = 1
    default segmented_value = 15
    default semicircle_value = 25
    default chain_chomp = 0
    default square_value = 55

    hbox:
        if bar_page == 0:
            ## EXAMPLE 1 : Pie Chart ###########################################
            ## This first bar has 4 segments and is shaped like a pie chart.
            circular_bar:
                value ScreenVariableValue('pie_value', 4)
                ## fore_bar is the bar image used beFORE it's full.
                fore_bar "b1_fore"
                ## aft_bar is the bar image used AFTer it's full.
                aft_bar "b1_aft"
                ## This makes the chart fill counter-clockwise.
                bar_invert True
                align (0.5, 0.5) xysize (649, 649)

            ## EXAMPLE 2 : Circle with Percent #################################
            ## This bar shows its current value as a percentage in the center.
            ## It goes from 0 to 100%.
            fixed:
                fit_first True align (0.5, 0.5)
                circular_bar:
                    ## You can provide many bar styles in a style, with a few
                    ## exceptions that are specified in intro blurb and where
                    ## relevant below.
                    ## Note that this bar has focus_mask True, which means the
                    ## player can only grab onto opaque parts of the bar.
                    style 'my_rad_bar_style'
                    ## start_thumb and its prefixed equivalents have to be
                    ## added on the screen. Here, start_thumb is used to round
                    ## off the start of the bar like the end.
                    ## Note that the colorize: displayable prefix is declared
                    ## at the bottom of this file.
                    start_thumb "colorize:b3_base2_thumb|#f93c3e"
                    hover_start_thumb "colorize:b3_base2_thumb|#FF8335"
                    ## This hides the start thumb when the bar is at 0 or 100%.
                    hide_start_thumb True
                    ## In this particular case, since we're using the same
                    ## images for the thumb and start_thumb, we could also have
                    ## used start_thumb True like below
                    # start_thumb True
                    value ScreenVariableValue('percentage_value', 100)
                ## This displays the current value of test_value as a percentage
                text "[percentage_value]%"

        elif bar_page == 1:
            ## EXAMPLE 3 : Segmented ###########################################
            ## This is an example of a bar image with gaps and an unusual
            ## number of segments.
            circular_bar:
                value ScreenVariableValue('segmented_value', 17)
                fore_bar "b2_fore"
                aft_bar "b2_aft"
                bar_invert True
                align (0.5, 0.5)
                xysize (649, 649)

            ## EXAMPLE 4 : Nested, Static ######################################
            ## These bars have static values, and can't be adjusted by the
            ## player. They also have start_angle -90, which means their start
            ## point is at 9:00 instead of 12:00.
            fixed:
                xysize (649, 649) align (0.5, 0.5)
                circular_bar:
                    value 94 range 100
                    fore_bar "colorize:b3_base|#21212d"
                    aft_bar At("b3_base", pretty_gradient)
                    start_angle -90
                    xysize (649, 649)
                circular_bar:
                    value 72 range 100
                    fore_bar "colorize:b3_base2|#21212d"
                    aft_bar At("b3_base2", pretty_gradient)
                    start_angle -90
                    xysize (649, 649)
                circular_bar:
                    value 48 range 100
                    fore_bar "colorize:b3_base3|#21212d"
                    aft_bar At("b3_base3", pretty_gradient)
                    start_angle -90
                    xysize (649, 649)

        elif bar_page == 2:
            ## EXAMPLE 5 : Half circle, rotating thumb #########################
            circular_bar:
                value ScreenVariableValue('semicircle_value', 90)
                ## These bar images are in a semi-circle from 9:00 to 3:00.
                ## Note, however, that the *center* of the circle is still the
                ## *center* of the image - this is so you don't have to specify
                ## the center of the circle yourself. So there is transparent
                ## space below the visible semi-circle.
                fore_bar "b5_fore" aft_bar "b5_aft"
                ## The focus mask is used to make only the top half of the bar
                ## focusable.
                focus_mask "b5_focus_mask"
                align (0.5, 0.5) xysize (649, 649)
                ## This thumb has thumb_rotate True so it rotates with the bar
                ## value.
                ## Thumbs are presumed to be at the right angle for a bar that
                ## is 0% full.
                thumb "b5_thumb" thumb_rotate True
                ## This bar deals with 180 degrees of a circle
                circumference 180
                ## and it starts at 9:00, which is -90 degrees from the default
                ## of 12:00.
                start_angle -90
                ## Note also that if bar_invert were True, this bar would be
                ## using the *bottom* half of the circle - i.e. 3:00 to 9:00 -
                ## instead of 9:00 to 3:00, since it would be going
                ## counter-clockwise.

            ## EXAMPLE 6 : Quarter circle, rotating thumb ######################
            circular_bar:
                value ScreenVariableValue('chain_chomp', 90)
                ## This bar image only uses a quarter of the circle for the
                ## active bar.
                fore_bar "b6_fore" aft_bar "b6_aft"
                ## This gutter prevents the thumb (the teeth) from going fully
                ## around the bar and creating an underbite. You can try
                ## changing this number to something from 0-89 to see what
                ## happens.
                aft_gutter 10
                align (0.5, 0.5) xysize (649, 649)
                ## The thumb is the teeth, which needs to rotate with the bar.
                thumb "b6_thumb" thumb_rotate True
                ## Only 90 degrees of the circle are used for the bar.
                circumference 90
                ## The bottom of the bar is 135 degrees from 12:00.
                start_angle 135
                ## The bar fills counter-clockwise.
                bar_invert True

        elif bar_page == 3:
            ## EXAMPLE 7 : Animated Bar ########################################
            ## This bar has an animated value that changes over time. It also
            ## uses base_bar and a thumb instead of fore and aft bars.
            circular_bar:
                value animated_adjustment
                base_bar "b7_base"
                thumb "b7_thumb" thumb_rotate True
                align (0.5, 0.5) xysize (649, 649)

            ## EXAMPLE 8 : Square bar ##########################################
            ## This bar is simply to demonstrate that bars may come in different
            ## shapes.
            circular_bar:
                value ScreenVariableValue('square_value', 100)
                fore_bar "#572D35" aft_bar "#f93c3e"
                bar_invert True start_angle 180
                align (0.5, 0.5) xysize (649, 649)
                ## This means there is no "buffer" so the bar will instantly
                ## go from 0 to 100% if dragged past the start.
                overshoot 0

    hbox:
        xalign 0.5 ycenter 0.9
        textbutton "Previous":
            selected False
            action [SetField(animated_adjustment, 'value', 60),
                SetField(animated_adjustment, 'start_time', None),
                SetScreenVariable('bar_page', (bar_page - 1) % max_bar_pages)]
        textbutton "Next":
            selected False
            action [SetField(animated_adjustment, 'value', 60),
                SetField(animated_adjustment, 'start_time', None),
                SetScreenVariable('bar_page', (bar_page + 1) % max_bar_pages)]

    textbutton "Return" action Return() align (1.0, 1.0)

style rbar_text:
    align (0.5, 0.5) size 80 color "#FFF"
    font "DejaVuSans.ttf"
style rbar_hbox:
    spacing (100 if config.screen_width > 1300 else 10)
    align (0.5, 0.5)

## EXAMPLE 2 : Circle with Percent #############################################
## This is the style for bar EXAMPLE 2. You can provide several properties
## here, like the bar images, size, and focus_mask. The following properties
## CANNOT be provided in a style, and must instead be declared on the circular_bar
## itself:
# overshoot - the "buffer" amount when dragging past 0% or 100% before the value
##            resets. Default is 0.25 aka 25% of the bar.
# start_angle - The number of degrees the start of the bar should be rotated. By
##           default it starts at 12:00. So, 90 would start it at 3:00.
# thumb_rotate - Can be set to True to have the thumb rotate with the bar.
# start_thumb - (and all its hover_ etc. counterparts). A displayable that's
##              permanently displayed at the start of the bar.
# hide_start_thumb - A boolean that hides the start_thumb when the bar is full
##                   or empty. Defaults to False.
## See the top of the file for more explanations on these properties.
style my_rad_bar_style:
    fore_bar "colorize:b3_base2|#21212d"
    aft_bar "colorize:b3_base2|#f93c3e"
    ## You can provide prefixes such as hover_ for bar images also.
    hover_aft_bar "colorize:b3_base2|#FF8335"
    focus_mask True
    xysize (649, 649)
    ## A thumb is provided to round off the end of the bar.
    thumb "colorize:b3_base2_thumb|#f93c3e"
    ## Thumbs also take prefixes
    hover_thumb "colorize:b3_base2_thumb|#FF8335"
    ## The center of the thumb should be thumb_offset pixels
    ## away from the center.
    thumb_offset absolute(238.5)


init -10 python:
    def quick_colorize(img):
        """
        A displayable prefix for shortening code to colorize black and white
        images to one solid colour.
        """
        img, color = img.split('|')
        return Transform(img, matrixcolor=ColorizeMatrix(color, color))

    config.displayable_prefix["colorize"] = quick_colorize

## If you have my gradient shaders, you can uncomment this version for pretty
## gradient colours: https://feniksdev.itch.io/gradients-for-renpy
# define 10 pretty_gradient = angle_gradient(["#FF00A5FF","#FA0952FF", "#FF4B62FF",
#         "#F9441AFF", "#FE8D32FF", "#FF944CFF", "#FFCD7CFF",
#         "#FFEFB4FF"], angle=-90, center=(0.5, 0.5))
## Otherwise this version does not require other plugins.
transform 10 pretty_gradient:
    matrixcolor ColorizeMatrix("#FF00A5FF", "#FF00A5FF")