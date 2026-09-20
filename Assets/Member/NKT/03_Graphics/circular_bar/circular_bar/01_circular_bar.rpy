################################################################################
##
## Circular Bar for Ren'Py by Feniks (feniksdev.itch.io / feniksdev.com)
##
################################################################################
## This file contains code for a circular bar displayable in Ren'Py. There is
## both a CDD to handle the rendering of the bar, and a screen language keyword
## so it can be easily declared in-game.
##
## If you use this code in your projects, credit me as Feniks @ feniksdev.com
##
## If you'd like to see how to use this tool, check the other file,
## circular_bar_examples.rpy! This is just the backend; you don't need to
## understand everything in this file.
##
## Leave a comment on the tool page on itch.io if you run into any issues.
################################################################################
## Code to archive these files for a distributed game. Do not remove.
init python:
    build.classify("**01_circular_bar.rpy", None)
    build.classify("**01_circular_bar.rpyc", "archive")
################################################################################
python early:
    import pygame, math

    class CircularBar(renpy.display.behavior.Bar):
        """
        A special kind of bar which displays in a circular arc.

        Unique Attributes:
        ------------------
        circumference : int
            The number of degrees the bar should take up. A number between 1
            and 360. Defaults to 360.
        start_angle : int
            The number of degrees the start of the bar should be rotated. Can
            be between -360 and 360. Defaults to 0, which means the bar begins
            at 12:00. Internally, this is saved under the name `rotation`.
        thumb_rotate : bool
            Whether or not the thumb should rotate with the bar. Defaults to
            False. If the thumb isn't rotating with the bar, you should
            typically provide a thumb_offset.
        thumb_offset : int
            The distance from the center of the bar to the center of the thumb.
            Defaults to 0.
        start_thumb : Displayable
            A displayable permanently displayed at the start of the bar. Obeys
            the offset rules as the thumb, but is otherwise expected to be
            already positioned at the start. Defaults to None. It also takes
            various prefixes (hover_, selected_hover_, etc.).
        hide_start_thumb : bool
            If True, when the bar is at 0 or 100%, the start thumb will be
            hidden. Defaults to False.
        overshoot : float
            The percent of the bar that acts as a buffer zone when going from
            0 to 100% or vice versa. Defaults to 0.25. This means that if you
            fill up the bar, you can drag up to 125% of the bar and it will
            remain at 100% until you lift your mouse and start dragging again.
            The same happens for dragging past 0%. A value of 0 means there is
            no buffer and the bar will jump from 0 to 100% and vice versa.
        focus_mask : displayable, True, callable, or None
            A mask that's used to control what portions of the button can be
            focused, and hence clicked on. The type of this property determines
            how it is interpreted. See:
            https://www.renpy.org/doc/html/style_properties.html#style-property-focus_mask
        bar_invert : bool
            If True, the bar will fill up counter-clockwise instead of
            clockwise. Defaults to False.
        """
        def __init__(self, *args, **kwargs):
            """Initialize the circular bar."""
            ## Replace style
            style = kwargs.pop('style', None)
            if style is None:
                style = "circular_bar"
            kwargs['style'] = style
            self.circumference = kwargs.pop('circumference', 360) % 360
            ## Circumference can't be 0
            if self.circumference == 0:
                self.circumference = 360
            ## The full circumference of the bar, before gutters are taken
            ## into account.
            self.full_circumference = self.circumference
            self.rotation = kwargs.pop('start_angle', 0) % 360
            ## The default calculations work off of 0 being 3:00, so this
            ## makes it so that 0 is 12:00.
            if self.rotation > 90:
                self.rotation -= 90
            else:
                self.rotation += 270
            self.thumb_rotate = kwargs.pop('thumb_rotate', False)
            self.overshoot = kwargs.pop('overshoot', 0.25)
            self.hide_start_thumb = kwargs.pop('hide_start_thumb', False)
            for k in list(kwargs.keys()):
                if "start_thumb" in k:
                    setattr(self, k, kwargs.pop(k))
            self.thumb_dimensions = None
            self.was_grabbed = False
            super(CircularBar, self).__init__(*args, **kwargs)

        def get_start_thumb(self):
            """
            Return the start thumb that should be used based on the bar's
            current state.
            """
            if getattr(self, "start_thumb", None) is True:
                return self.style.thumb

            prefix = self.style.prefix
            if prefix == "selected_hover_":
                poss = ["selected_hover_", "hover_", "selected_", ""]
            elif prefix == "selected_idle_":
                poss = ["selected_idle_", "idle_", "selected_", ""]
            elif prefix == "selected_":
                poss = ["selected_", ""]
            elif prefix == "insensitive":
                poss = ["insensitive_", "", "idle_"]
            elif prefix == "hover_":
                poss = ["hover_", ""]
            elif prefix == "idle_":
                poss = ["idle_", ""]
            else:
                poss = [""]
            for p in poss:
                if hasattr(self, p + "start_thumb"):
                    return renpy.easy.displayable(getattr(self, p + "start_thumb"))
            return None

        def render(self, width, height, st, at):
            """
            Render the circular bar. Much of this code comes from the built-in
            Bar class in renpy/display/behavior.rpy.
            """

            # Handle redrawing.
            if self.value is not None:
                redraw = self.value.periodic(st)

                if redraw is not None:
                    renpy.display.render.redraw(self, redraw)

            xminimum, yminimum = renpy.display.layout.xyminimums(self.style,
                width, height)

            if xminimum is not None:
                width = max(width, xminimum)

            if yminimum is not None:
                height = max(height, yminimum)

            # Store the width and height for the event function to use.
            self.width = width
            self.height = height
            range = self.adjustment.range # @ReservedAssignment
            value = self.adjustment.value
            page = self.adjustment.page

            if range <= 0:
                if self.style.unscrollable == "hide":
                    self.hidden = True
                    return renpy.display.render.Render(width, height)
                elif self.style.unscrollable == "insensitive":
                    self.set_style_prefix("insensitive_", True)
            else:
                if self.style.prefix == "insensitive_":
                    self.set_style_prefix("idle_", True)

            self.hidden = False

            fore_gutter = self.style.fore_gutter
            aft_gutter = self.style.aft_gutter

            self.circumference = self.full_circumference - fore_gutter - aft_gutter

            thumb_dim = min(self.width, self.height)
            thumb_offset = abs(self.style.thumb_offset)

            thumb = renpy.render(self.style.thumb, thumb_dim, height, st, at)
            self.og_thumb_w, self.og_thumb_h = thumb.get_size()
            thumb_shadow = renpy.render(self.style.thumb_shadow, thumb_dim, height, st, at)

            rv = renpy.display.render.Render(width, height)

            invert = self.style.bar_invert
            rotation = self.style.fore_gutter + self.rotation

            ## Get the point that is thumb_offset from the center of the bar
            ## at angle degrees.
            angle = math.radians(float(self.adjustment.value) / float(self.adjustment.range) * float(self.circumference))
            ## Where do we put the thumb?
            self.last_thumb_angle = 0
            if self.thumb_rotate:
                if invert:
                    thumb_angle = int(math.degrees(-angle))
                else:
                    thumb_angle = int(math.degrees(angle))
                self.last_thumb_angle = thumb_angle
                thumb = renpy.render(Transform(self.style.thumb,
                        rotate_pad=False, rotate=thumb_angle),
                    min(self.width, self.height), height, st, at)

            else:
                thumb = renpy.render(self.style.thumb,
                    min(self.width, self.height), height, st, at)
            self.thumb_w, self.thumb_h = thumb.get_size()

            ## The offset is how far from the center of the bar the thumb is.
            thumb_offset = abs(self.style.thumb_offset)
            if invert:
                angle = -angle
            thumb_pos_offset = math.radians(360-rotation)
            x = math.cos(angle-thumb_pos_offset) * thumb_offset
            y = math.sin(angle-thumb_pos_offset) * thumb_offset

            ## Blit the thumb shadow
            rv.blit(thumb_shadow, (width/2.0 + x - thumb_shadow.width/2.0,
                height/2.0 + y - thumb_shadow.height/2.0))

            ## Render the bar
            if invert: # Counter clockwise
                tex0 = self.style.aft_bar
                tex1 = self.style.fore_bar
            else:
                tex0 = self.style.fore_bar
                tex1 = self.style.aft_bar

            pct = float(self.adjustment.value) / float(self.adjustment.range)
            ## Consider the circumference
            actual_bar_used = self.circumference / 360.0
            pct *= actual_bar_used

            if self.adjustment.value == 0 and self.circumference == 360:
                rv.blit(renpy.render(self.style.fore_bar, width, height, st, at), (0, 0))
            elif (self.adjustment.value == self.adjustment.range
                    and self.circumference == 360):
                rv.blit(renpy.render(self.style.aft_bar, width, height, st, at), (0, 0))
            else:
                ## This uses a shader to wipe around the bar.
                child = Model().child(tex0, fit=True).texture(tex1)
                child.mesh = True
                ## Fun fact: this is extremely similar to the shader used
                ## for angular gradients in my gradient collection on itch.
                child.shader("feniks.circular_wipe")
                child.uniform("u_angle", (rotation + 180
                    if rotation < 180 else rotation - 180))
                if invert:
                    child.uniform("u_value", 1.0 - pct)
                else:
                    child.uniform("u_value", pct)

                child.property("mipmap", config.mipmap_dissolves if (
                    self.style.mipmap is None) else self.style.mipmap)
                rv.blit(child.render(width, height, st, at), (0, 0))

            start_thumb = self.get_start_thumb()
            if self.hide_start_thumb and (self.adjustment.value == 0
                    or self.adjustment.value == self.adjustment.range):
                ## Don't show the start thumb when it's precisely overlapping
                ## with the thumb.
                start_thumb = None
            if start_thumb:
                ## Blit the start thumb
                start_angle = math.radians(self.rotation)
                start_x = math.cos(start_angle) * thumb_offset
                start_y = math.sin(start_angle) * thumb_offset
                start_thumb = renpy.render(start_thumb, min(self.width, self.height), height, st, at)
                rv.blit(start_thumb, (width/2.0 + start_x - start_thumb.width/2.0,
                    height/2.0 + start_y - start_thumb.height/2.0))

            ## Blit the thumb
            self.thumb_dimensions = (width/2.0 + x - thumb.width/2.0,
                height/2.0 + y - thumb.height/2.0, thumb.width, thumb.height)
            rv.blit(thumb, (width/2.0 + x - thumb.width/2.0,
                height/2.0 + y - thumb.height/2.0))

            ## Check the focus mask.
            if self.focusable:
                mask = self.style.focus_mask
                if mask is True:
                    mask = rv
                elif mask is not None:
                    try:
                        mask = renpy.display.render.render(mask, rv.width, rv.height, st, at)
                    except Exception:
                        if callable(mask):
                            mask = mask
                        else:
                            raise Exception("Focus_mask must be None, True, a displayable, or a callable.")

                if mask is not None:
                    fmx = 0
                    fmy = 0
                else:
                    fmx = None
                    fmy = None
                rv.add_focus(self, None, 0, 0, width, height, fmx, fmy, mask)

            return rv

        def grab_bar(self):
            """Indicate the bar has been grabbed."""
            renpy.display.tts.speak(renpy.minstore.__("activate"))
            self.set_style_prefix("selected_hover_", True)
            renpy.play(self.style.activate_sound)
            self.was_grabbed = True

        def event(self, ev, x, y, st):
            """
            Handle events for the circular bar. Borrows largely from the Bar
            class in renpy/display/behavior.rpy.
            """

            if not self.focusable:
                return None

            if not self.is_focused():
                return None

            if self.hidden:
                return None

            range = self.adjustment.range # @ReservedAssignment
            old_value = self.adjustment.value
            value = old_value

            invert = self.style.bar_invert

            grabbed = (renpy.display.focus.get_grab() is self)
            just_grabbed = False

            ignore_event = False

            if not grabbed and renpy.map_event(ev, "bar_activate"):
                renpy.display.focus.set_grab(self)
                just_grabbed = True
                grabbed = True
                ignore_event = True

            if grabbed:
                ## Since controllers don't have a great analog to dragging in a
                ## circle/using the thumbstick would be awkward, they can just
                ## select the bar and hit up/down.
                increase = "bar_up"
                decrease = "bar_down"

                if renpy.map_event(ev, decrease):
                    renpy.display.tts.speak(renpy.minstore.__("decrease"))
                    value -= self.adjustment.step
                    ignore_event = True

                if renpy.map_event(ev, increase):
                    renpy.display.tts.speak(renpy.minstore.__("increase"))
                    value += self.adjustment.step
                    ignore_event = True

                if ev.type in (pygame.MOUSEMOTION, pygame.MOUSEBUTTONUP,
                        pygame.MOUSEBUTTONDOWN):
                    ## Calculate the angle from the center of the bar to the
                    ## mouse position.
                    angle = math.atan2(y - self.height/2.0, x - self.width/2.0)
                    if (angle < 0):
                        angle += 2.0 * math.pi;
                    ## Adjust for bar rotation
                    ## Imagine the bar's rotation is 90, so it starts at 3:00
                    ## The angle provided is relative to 12:00, so we need to
                    ## subtract 90 degrees to get the angle relative to 3:00.
                    angle -= math.radians(self.rotation+self.style.fore_gutter)
                    if invert:
                        angle = -angle
                    ## Normalize it to an angle between 0 and 2pi
                    if (angle < 0):
                        angle += 2.0 * math.pi;
                    ## The angle is at most relative to the circumference and
                    ## at least 0.
                    invalid_area = False
                    if angle < 0 or angle > math.radians(self.circumference):
                        invalid_area = True
                    angle = max(0, min(angle, math.radians(self.circumference)))

                    ## Calculate the percent of the bar the angle is, based on
                    ## the circumference.
                    percent = angle / math.radians(self.circumference)
                    old_percent = old_value / float(self.adjustment.range)

                    ## Did they grab the thumb?
                    grabbed_thumb = False
                    if (ev.type == pygame.MOUSEBUTTONDOWN):
                        if (not self.thumb_rotate
                                and self.thumb_dimensions is not None
                                and x >= self.thumb_dimensions[0]
                                and x <= self.thumb_dimensions[0] + self.thumb_dimensions[2]
                                and y >= self.thumb_dimensions[1]
                                and y <= self.thumb_dimensions[1] + self.thumb_dimensions[3]):
                            ## Yes; don't overshoot
                            grabbed_thumb = True
                        else:
                            ## Check for transparency; we have to offset based
                            ## on rotation.
                            try:
                                w_offset = (self.og_thumb_w - self.thumb_w) / 2.0
                                h_offset = (self.og_thumb_h - self.thumb_h) / 2.0
                                grabbed_thumb = renpy.is_pixel_opaque(
                                    Transform(self.style.thumb,
                                        rotate=self.last_thumb_angle,
                                        rotate_pad=False),
                                    self.og_thumb_w, self.og_thumb_h, st, st,
                                    x-w_offset, y-h_offset)
                            except Exception as e:
                                grabbed_thumb = False

                    did_not_grab = False

                    ## Don't jump from 0 to 100%
                    if self.overshoot <= 0:
                        value = percent * range
                    elif ((ev.type != pygame.MOUSEBUTTONDOWN or grabbed_thumb)
                            and percent > (1.0 - self.overshoot)
                            and old_percent < self.overshoot):
                        value = 0
                    ## Don't jump from 100% to 0%
                    elif ((ev.type != pygame.MOUSEBUTTONDOWN or grabbed_thumb)
                            and percent < self.overshoot
                            and old_percent > (1.0 - self.overshoot)):
                        value = range
                    elif ((ev.type == pygame.MOUSEBUTTONDOWN
                            and not grabbed_thumb
                            and invalid_area)):
                        did_not_grab = True
                    elif invalid_area and not grabbed_thumb and not self.was_grabbed:
                        ## A mouse up or movement event outside the valid area
                        ## when it wasn't grabbed; do not adjust the value.
                        did_not_grab = True
                    else:
                        value = percent * range

                    ignore_event = True

                    if just_grabbed and not did_not_grab:
                        self.grab_bar()

                if isinstance(range, int):
                    value = round(value)

                if value < 0:
                    renpy.display.tts.speak("")
                    value = 0

                if value > range:
                    renpy.display.tts.speak("")
                    value = range

            ## If it was grabbed but is now no longer being grabbed, release it.
            if (grabbed and not just_grabbed
                    and renpy.map_event(ev, "bar_deactivate")):
                was_grabbed = False
                if self.was_grabbed:
                    was_grabbed = True
                    self.was_grabbed = False
                    renpy.display.tts.speak(renpy.minstore.__("deactivate"))
                self.set_style_prefix("hover_", True)
                renpy.display.focus.set_grab(None)

                if not was_grabbed:
                    ## They grabbed outside the valid area; don't change the
                    ## value or run any release events.
                    raise renpy.IgnoreEvent()

                # Invoke rounding adjustment on bar release
                value = self.adjustment.round_value(value, release=True)
                if value != old_value:
                    rv = self.adjustment.change(value)
                    if rv is not None:
                        return rv

                rv = renpy.run(self.released)
                if rv is not None:
                    return rv

                raise renpy.IgnoreEvent()

            if value != old_value:
                value = self.adjustment.round_value(value, release=False)
                rv = self.adjustment.change(value)
                if rv is not None:
                    return rv

            if ignore_event:
                raise renpy.IgnoreEvent()
            else:
                return None

    ## Register this bar as a displayable.
    renpy.register_sl_displayable("circular_bar", CircularBar, 'circular_bar', 0,
        replaces=True, pass_context=True
    ).add_property("adjustment"
    ).add_property("range"
    ).add_property("value"
    ).add_property("changed"
    ).add_property("hovered"
    ).add_property("unhovered"
    ).add_property("released"
    ).add_property("circumference"
    ).add_property("overshoot"
    ).add_property("start_angle"
    ).add_property("thumb_rotate"
    ).add_property("focus_mask"
    ).add_style_property("fore_bar"
    ).add_property("fore_gutter"
    ).add_style_property("aft_bar"
    ).add_property("aft_gutter"
    ).add_property("activate_sound"
    ).add_property("hover_sound"
    ).add_style_property("start_thumb"
    ).add_property("hide_start_thumb"
    ).add_property_group("bar")

init python:
    ############################################################################
    ## SHADER
    ############################################################################
    ## This is a shader used to wipe around the circular bar from the center.
    ## u_angle is the angle of the start of the bar (default is at 12:00),
    ## in degrees.
    ## u_value is the value of the bar, from 0 to 1. 0 will use 100% of tex0,
    ## 1 will use 100% of tex1.
    renpy.register_shader("feniks.circular_wipe", variables="""
        uniform float u_lod_bias;
        uniform float u_angle;
        uniform float u_value;
        uniform sampler2D tex0;
        uniform sampler2D tex1;
        uniform vec2 u_model_size;
        attribute vec4 a_position;
        varying vec2 v_coords;
        varying vec2 v_size;
    """, vertex_300="""
        v_coords = vec2(a_position.x / u_model_size.x, a_position.y / u_model_size.y);
        v_size = u_model_size;
    """, fragment_300="""
        vec2 center = vec2(0.5);
        float PI = 3.1415926535897932384626433832795;
        float pixel_size = max(1.0 / v_size.x, 1.0 / v_size.y) * 0.5;

        // Move the whole thing so the center is at (0, 0) for calculations
        vec2 new_coords = v_coords - center;
        // Use atan to determine the angle, and multiply by radius to get the
        // arc length (but it cancels out later so no radius).
        float arc_length = (atan(new_coords.y, new_coords.x) + PI);
        // num is the % of that
        // Circumference = 2*pi*r -> radius cancelled
        float num = min(arc_length / (2.0 * PI), 1.0);

        // Add to num to account for the provided angle.
        // The angle rotates counter-clockwise and we want to rotate clockwise
        num += (2.0*PI - radians(u_angle)) / (2.0*PI);
        if (num > 1.0 || num < 0.0) {
            num = mod(num, 1.0);
        }
        num = max(min(num, 1.0), 0.0);

        // Smoothstep the transition between the two textures to
        // avoid a jagged line.
        gl_FragColor = mix(texture2D(tex1, v_coords, u_lod_bias),
            texture2D(tex0, v_coords, u_lod_bias),
            smoothstep(max(u_value-pixel_size, 0.0),
                    min(u_value+pixel_size, 1.0), num));
    """)


## The basic style for any circular bar. All circular bars will inherit
## from this base style.
style circular_bar:
    is bar