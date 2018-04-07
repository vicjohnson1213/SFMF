$(function () {
    $.get('https://raw.githubusercontent.com/vicjohnson1213/SFMF/manifest/manifest.json')
        .then((res) => {
            let sfmf = JSON.parse(res);

            $('#download-button').text(`Download ${sfmf.version}`)

            sfmf.modlist
                .filter(mod => mod.featured)
                .slice(0,3)
                .forEach(mod => {
                    $('#mod-list').append(createMod(mod));
                });
        });

    function getRandomColor() {
        colors = [
            'navy',
            'green',
            'teal',
            'blue',
            'purple',
            'pink',
            'red',
            'orange',
            'yellow'
        ];

        return colors[Math.floor(colors.length * Math.random())];
    }

    function createMod(mod) {
        let col = $('<div>', {
            class: 'siimple-grid-col siimple-grid-col--4 siimple-grid-col-sm--12'
        });

        let header = $('<div>', {
            class: 'siimple-h3',
            text: mod.name
        });

        let description = $('<div>', {
            class: 'siimple-p',
            text: mod.description
        });

        col.append(header);
        col.append(description);

        // let box = $('<div>', {
        //     class: 'siimple-box mod-box'
        // });
        //
        // let header = $('<div>', {
        //     class: 'siimple-box mod-box-header'
        // }).addClass('siimple-box--' + getRandomColor());
        //
        // let headerTitle = $('<div>', {
        //     class: 'siimple-box-title',
        //     text: mod.name
        // });
        //
        // let body = $('<div>', {
        //     class: 'mod-box-body',
        //     text: mod.description
        // });
        //
        // let version = $('<div>', {
        //     class: 'mod-box-version',
        //     text: mod.version
        // });

        // col.append(box.append(header.append(headerTitle)));
        // box.append(body);
        // box.append(version);

        return col;
    }
})
