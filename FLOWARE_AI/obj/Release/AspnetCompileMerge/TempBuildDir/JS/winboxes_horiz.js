let openWinBoxes = {};

function PopUpIframe(PrmTitle, PrmWidth, PrmHeight, classFrame, path) {
    // ✅ Reuse if already open
    if (openWinBoxes[classFrame] && !openWinBoxes[classFrame].closed) {
        const winBox = openWinBoxes[classFrame];
        if (winBox.min) winBox.minimize(false);
        winBox.focus();
        return;
    }
    const iframe = document.createElement("iframe");
    iframe.src = path;
    iframe.style.width = "100%";
    iframe.style.height = "100%";
    iframe.style.border = "0";
    
    function getResponsiveSize() {
        if (window.innerWidth <= 768) {
            // 📱 Phones → Fullscreen
            return {
                width: window.innerWidth + "px",
                height: window.innerHeight + "px",
                move: false,
                resize: false
            };
        } else if (window.innerWidth <= 1200) {
            // 📲 Tablets (portrait & landscape) → Fullscreen
            return {
                width: window.innerWidth + "px",
                height: window.innerHeight + "px",
                move: false,
                resize: false,
                fullscreen: false, // ❌ don't hide header
                max: true
            };
        } else {
            // 💻 Desktops / Large laptops
            return {
                width: PrmWidth ? PrmWidth : Math.floor(window.innerWidth * 0.7) + "px",
                height: PrmHeight ? PrmHeight : Math.floor(window.innerHeight * 0.8) + "px",
                move: true,
                resize: true
            };
        }
    }

    // Initial size
    let size = getResponsiveSize();

    const wb = new WinBox(PrmTitle, {
        class: classFrame,
        width: size.width,
        height: size.height,
        x: "center",
        y: "center",
        mount: iframe,
        move: size.move,
        resize: size.resize,
        oncreate() {
            this.focus();
        },
        onclose(force) {
            console.log(`Closed: ${PrmTitle}`);

            // Only run if jQuery exists and the modal is present
            if (window.$ && $("#mi-modal").length) {
                $("#mi-modal").modal("show");
                $("#FrameClass").val(classFrame);
            }

            delete openWinBoxes[classFrame];

            return false; // allow WinBox to close[web:3][web:6]
        }
    });

    openWinBoxes[classFrame] = wb;

    // ✅ Auto-resize on rotation or window resize
    function handleResize() {
        if (!wb.closed) {
            let newSize = getResponsiveSize();
            wb.resize(newSize.width, newSize.height);
            wb.move("center", "center");
        }
    }

    window.addEventListener("resize", handleResize);
    window.addEventListener("orientationchange", handleResize);
}


//------For Left Alignment 

//function PopUpIframeLeft(PrmTitle, PrmWidth, PrmHeight, classFrame, path) {
//    // ✅ Reuse if already open
//    if (openWinBoxes[classFrame] && !openWinBoxes[classFrame].closed) {
//        const winBox = openWinBoxes[classFrame];
//        if (winBox.min) winBox.minimize(false);
//        winBox.focus();
//        return;
//    }
//    //const iframe = document.createElement("iframe");
//    //iframe.src = path;
//    //iframe.style.width = "100%";
//    //iframe.style.height = "100%";
//    //iframe.style.border = "0";
//    const iframe = document.createElement("iframe");
//    iframe.src = path + "#zoom=page-width";
//    iframe.style.width = "100%";
//    iframe.style.height = "100%";
//    iframe.style.border = "0";
//    function getResponsiveSize() {
//        if (window.innerWidth <= 768) {
//            // 📱 Phones → Fullscreen
//            return {
//                width: window.innerWidth + "px",
//                height: window.innerHeight + "px",
//                move: false,
//                resize: false
//            };
//        } else if (window.innerWidth <= 1200) {
//            // 📲 Tablets (portrait & landscape) → Fullscreen
//            return {
//                width: window.innerWidth + "px",
//                height: window.innerHeight + "px",
//                move: false,
//                resize: false,
//                fullscreen: false, // ❌ don't hide header
//                max: true
//            };
//        } else {
//            // 💻 Desktops / Large laptops
//            return {
//                width: PrmWidth ? PrmWidth : Math.floor(window.innerWidth * 0.7) + "px",
//                height: PrmHeight ? PrmHeight : Math.floor(window.innerHeight * 0.8) + "px",
//                move: true,
//                resize: true
//            };
//        }
//    }

//    // Initial size
//    let size = getResponsiveSize();

//    const wb = new WinBox(PrmTitle, {
//        class: classFrame,
//        width: size.width,
//        height: size.height,
//        x: 2,
//        y: 75,
//        mount: iframe,
//        move: size.move,
//        resize: size.resize,
//        oncreate() {
//            this.focus();
//        },
//        onclose(force) {
//            //console.log(`Closed: ${PrmTitle}`);

//            //// Only run if jQuery exists and the modal is present
//            //if (window.$ && $("#mi-modal").length) {
//            //    $("#mi-modal").modal("show");
//            //    $("#FrameClass").val(classFrame);
//            //}

//            //delete openWinBoxes[classFrame];

//            //return false; // allow WinBox to close[web:3][web:6]

//            console.log(`Closed: ${PrmTitle}`);

//            if (window.$ && $("#mi-modal").length) {
//                $("#mi-modal").modal("show");
//                $("#FrameClass").val(classFrame);
//            }

//            delete openWinBoxes[classFrame];

//            return true;
//        }
//    });

//    openWinBoxes[classFrame] = wb;

//    // ✅ Auto-resize on rotation or window resize
//    function handleResize() {
//        if (!wb.closed) {
//            let newSize = getResponsiveSize();
//            wb.resize(newSize.width, newSize.height);
//            wb.move("center", "center");
//        }
//    }

//    window.addEventListener("resize", handleResize);
//    window.addEventListener("orientationchange", handleResize);
//}

function PopUpIframeLeft(PrmTitle, PrmWidth, PrmHeight, classFrame, path) {

    // Reuse if already open
    if (openWinBoxes[classFrame] && !openWinBoxes[classFrame].closed) {
        const winBox = openWinBoxes[classFrame];

        if (winBox.min) {
            winBox.minimize(false);
        }

        winBox.focus();
        return;
    }

    // Create iframe
    const iframe = document.createElement("iframe");

    iframe.src = path + "#zoom=page-width";

    iframe.style.width = "100%";
    iframe.style.height = "100%";
    iframe.style.border = "0";
    iframe.style.display = "block";


    // Responsive size
    function getResponsiveSize() {

        if (window.innerWidth <= 768) {

            // Phones
            return {
                width: window.innerWidth + "px",
                height: window.innerHeight + "px",
                move: false,
                resize: false
            };

        } else if (window.innerWidth <= 1200) {

            // Tablets
            return {
                width: window.innerWidth + "px",
                height: window.innerHeight + "px",
                move: false,
                resize: false,
                fullscreen: false,
                max: true
            };

        } else {

            // Desktop
            return {
                width: PrmWidth
                    ? PrmWidth
                    : Math.floor(window.innerWidth * 0.7) + "px",

                height: PrmHeight
                    ? PrmHeight
                    : Math.floor(window.innerHeight * 0.8) + "px",

                move: true,
                resize: true
            };
        }
    }


    // Initial size
    let size = getResponsiveSize();


    // Create WinBox
    const wb = new WinBox(PrmTitle, {

        class: classFrame,

        width: size.width,
        height: size.height,

        // Left position
        x: 2,
        y: 75,

        mount: iframe,

        move: size.move,
        resize: size.resize,

        oncreate() {
            this.focus();
        },

        onclose(force) {

            console.log(`Closed: ${PrmTitle}`);

            if (window.$ && $("#mi-modal").length) {
                $("#mi-modal").modal("show");
                $("#FrameClass").val(classFrame);
            }

            delete openWinBoxes[classFrame];

            return true;
        }
    });


    openWinBoxes[classFrame] = wb;


    // Auto resize
    function handleResize() {

        if (!wb.closed) {

            let newSize = getResponsiveSize();

            wb.resize(
                newSize.width,
                newSize.height
            );

            // Keep WinBox on LEFT
            wb.move(2, 75);
        }
    }


    window.addEventListener(
        "resize",
        handleResize
    );

    window.addEventListener(
        "orientationchange",
        handleResize
    );
}











