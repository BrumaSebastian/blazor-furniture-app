export function onWheel(element, deltaY) {
  if (!element) return;
  element.scrollLeft += deltaY;
}
