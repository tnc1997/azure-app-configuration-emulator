// https://github.com/microsoft/fast/blob/%40microsoft/fast-foundation_v2.50.0/packages/web-components/fast-foundation/src/anchored-region/anchored-region.ts

var AXIS_POSITIONING_MODES = {
    uncontrolled: 0,
    lockToDefault: 1,
    dynamic: 2,
};

var HORIZONTAL_POSITIONS = {
    unset: 0,
    left: 1,
    right: 2,
    center: 3,
};

var VERTICAL_POSITIONS = {
    unset: 0,
    top: 1,
    bottom: 2,
    center: 3,
};

export function anchor(element, anchor, horizontalDefaultPosition, horizontalInset, horizontalPositioningMode, horizontalThreshold, verticalDefaultPosition, verticalInset, verticalPositioningMode, verticalThreshold) {
    anchor = document.getElementById(anchor);

    var horizontalPosition = getHorizontalPosition(element, anchor, horizontalDefaultPosition, horizontalInset, horizontalPositioningMode, horizontalThreshold);
    var verticalPosition = getVerticalPosition(element, anchor, verticalDefaultPosition, verticalInset, verticalPositioningMode, verticalThreshold);

    var x = getHorizontalOffset(element, anchor, horizontalPosition, horizontalInset, horizontalPositioningMode);
    var y = getVerticalOffset(element, anchor, verticalPosition, verticalInset, verticalPositioningMode);

    element.style.transform = `translate(${x}px, ${y}px)`;
}

function getHorizontalOffset(element, anchor, horizontalPosition, horizontalInset, horizontalPositioningMode) {
    switch (horizontalPositioningMode) {
        case AXIS_POSITIONING_MODES.uncontrolled:
        default:
            return 0;
        case AXIS_POSITIONING_MODES.lockToDefault:
        case AXIS_POSITIONING_MODES.dynamic:
            switch (horizontalPosition) {
                case HORIZONTAL_POSITIONS.unset:
                case HORIZONTAL_POSITIONS.right:
                default:
                    if (horizontalInset === true) {
                        return 0;
                    } else {
                        return anchor.offsetWidth;
                    }
                case HORIZONTAL_POSITIONS.left:
                    if (horizontalInset === true) {
                        return anchor.offsetWidth - element.offsetWidth;
                    } else {
                        return 0 - element.offsetWidth;
                    }
                case HORIZONTAL_POSITIONS.center:
                    return (anchor.offsetWidth - element.offsetWidth) / 2;
            }

            break;
    }
}

function getHorizontalPosition(element, anchor, horizontalDefaultPosition, horizontalInset, horizontalPositioningMode, horizontalThreshold) {
    switch (horizontalPositioningMode) {
        case AXIS_POSITIONING_MODES.uncontrolled:
        default:
            return HORIZONTAL_POSITIONS.unset;
        case AXIS_POSITIONING_MODES.lockToDefault:
            return horizontalDefaultPosition;
        case AXIS_POSITIONING_MODES.dynamic:
            var horizontalOffset = getHorizontalOffset(element, anchor, horizontalDefaultPosition, horizontalInset, horizontalPositioningMode);

            switch (horizontalDefaultPosition) {
                case HORIZONTAL_POSITIONS.unset:
                case HORIZONTAL_POSITIONS.right:
                default:
                    if (horizontalInset === true) {
                        if (horizontalOffset < 0 + horizontalThreshold) {
                            return HORIZONTAL_POSITIONS.left;
                        } else {
                            return horizontalDefaultPosition;
                        }
                    } else {
                        if (horizontalOffset + element.offsetWidth > window.innerWidth - horizontalThreshold) {
                            return HORIZONTAL_POSITIONS.left;
                        } else {
                            return horizontalDefaultPosition;
                        }
                    }
                case HORIZONTAL_POSITIONS.left:
                    if (horizontalInset === true) {
                        if (horizontalOffset + element.offsetWidth > window.innerWidth - horizontalThreshold) {
                            return HORIZONTAL_POSITIONS.right;
                        } else {
                            return horizontalDefaultPosition;
                        }
                    } else {
                        if (horizontalOffset < 0 + horizontalThreshold) {
                            return HORIZONTAL_POSITIONS.right;
                        } else {
                            return horizontalDefaultPosition;
                        }
                    }
                case HORIZONTAL_POSITIONS.center:
                    return horizontalDefaultPosition;
            }

            break;
    }
}

function getVerticalOffset(element, anchor, verticalPosition, verticalInset, verticalPositioningMode) {
    switch (verticalPositioningMode) {
        case AXIS_POSITIONING_MODES.uncontrolled:
        default:
            return 0;
        case AXIS_POSITIONING_MODES.lockToDefault:
        case AXIS_POSITIONING_MODES.dynamic:
            switch (verticalPosition) {
                case VERTICAL_POSITIONS.unset:
                case VERTICAL_POSITIONS.top:
                default:
                    if (verticalInset === true) {
                        return 0 - element.offsetHeight;
                    } else {
                        return 0 - (anchor.offsetHeight + element.offsetHeight);
                    }
                case VERTICAL_POSITIONS.bottom:
                    if (verticalInset === true) {
                        return 0 - anchor.offsetHeight;
                    } else {
                        return 0;
                    }
                case VERTICAL_POSITIONS.center:
                    return 0 - (anchor.offsetHeight - (anchor.offsetHeight - element.offsetHeight) / 2);
            }

            break;
    }
}

function getVerticalPosition(element, anchor, verticalDefaultPosition, verticalInset, verticalPositioningMode, verticalThreshold) {
    switch (verticalPositioningMode) {
        case AXIS_POSITIONING_MODES.uncontrolled:
        default:
            return VERTICAL_POSITIONS.unset;
        case AXIS_POSITIONING_MODES.lockToDefault:
            return verticalDefaultPosition;
        case AXIS_POSITIONING_MODES.dynamic:
            var verticalOffset = getVerticalOffset(element, anchor, verticalDefaultPosition, verticalInset, verticalPositioningMode);

            switch (verticalDefaultPosition) {
                case VERTICAL_POSITIONS.unset:
                case VERTICAL_POSITIONS.top:
                default:
                    if (verticalInset === true) {
                        if (verticalOffset < 0 + verticalThreshold) {
                            return VERTICAL_POSITIONS.bottom;
                        } else {
                            return verticalDefaultPosition;
                        }
                    } else {
                        if (verticalOffset + element.offsetHeight > window.innerHeight - verticalThreshold) {
                            return VERTICAL_POSITIONS.bottom;
                        } else {
                            return verticalDefaultPosition;
                        }
                    }
                case VERTICAL_POSITIONS.bottom:
                    if (verticalInset === true) {
                        if (verticalOffset < 0 + verticalThreshold) {
                            return VERTICAL_POSITIONS.top;
                        } else {
                            return verticalDefaultPosition;
                        }
                    } else {
                        if (verticalOffset + element.offsetHeight > window.innerHeight - verticalThreshold) {
                            return VERTICAL_POSITIONS.top;
                        } else {
                            return verticalDefaultPosition;
                        }
                    }
                case VERTICAL_POSITIONS.center:
                    return verticalDefaultPosition;
            }

            break;
    }
}
