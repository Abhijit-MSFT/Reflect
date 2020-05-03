var divEle = document.getElementById("selectedTxt");
function getSelectedOption(self) {
  divEle.textContent = self.options[self.selectedIndex].text;
}
