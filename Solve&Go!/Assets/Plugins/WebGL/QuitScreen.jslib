// Browser side of the Quit button (called from MainMenu.QuitGame).
// A web page can't close its own tab, so this hides the game and shows a
// "Thanks for playing" screen with a button that reloads the game.
mergeInto(LibraryManager.library, {
  SolveGo_ShowQuitScreen: function () {
    if (document.getElementById('solvego-quit')) return;

    if (document.fullscreenElement && document.exitFullscreen) {
      document.exitFullscreen();
    }

    var container = document.getElementById('unity-container');
    if (container) container.style.visibility = 'hidden';

    var screen = document.createElement('div');
    screen.id = 'solvego-quit';
    screen.style.cssText =
      'position:fixed;left:0;top:0;right:0;bottom:0;z-index:9999;' +
      'display:flex;flex-direction:column;align-items:center;justify-content:center;' +
      'background:#07070d;font-family:Arial,Helvetica,sans-serif;text-align:center;';
    screen.innerHTML =
      '<div style="font-size:56px;font-weight:bold;color:#3ad1ff;letter-spacing:2px;' +
      'text-shadow:0 0 8px #00b4ff,0 0 28px #00b4ff;">Thanks for playing!</div>' +
      '<div style="margin-top:18px;font-size:16px;color:#ff5ce6;letter-spacing:5px;">' +
      'YOU CAN CLOSE THIS TAB NOW</div>' +
      '<button id="solvego-again" style="margin-top:44px;padding:14px 38px;font-size:22px;' +
      'font-weight:bold;letter-spacing:4px;color:#07070d;background:#3ad1ff;border:0;' +
      'border-radius:10px;cursor:pointer;box-shadow:0 0 18px #00b4ff;">PLAY AGAIN</button>';
    document.body.appendChild(screen);

    document.getElementById('solvego-again').onclick = function () {
      window.location.reload();
    };
  }
});
