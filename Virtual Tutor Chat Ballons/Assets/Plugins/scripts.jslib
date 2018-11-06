mergeInto(LibraryManager.library, {

  HelloString: function (str) {
    var line = Pointer_stringify(str);
    window.parent.getTopic(line);
  },

  GetAF: function (str) {
    var line = Pointer_stringify(str);
    window.parent.getAF(line);
  },

  ItExists: function (str) {
    var line = Pointer_stringify(str);
    return window.parent.getElemPage(line) != null;
  },

  GoToGradeRep: function () {
    return window.parent.goToGradeReport();
  }
});