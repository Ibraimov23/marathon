function onlyOne(checkbox) {
    var checkboxes = document.getElementsByName('roles')
    checkboxes.forEach((item) => {
        if (item !== checkbox) item.checked = false
    })
}