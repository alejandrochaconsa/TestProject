async function getItems(path){
    try{
        reloadBreadCrumbs(path);
        
        const response = await fetch(`/api/v1/items?path=${encodeURIComponent(path)}`, { method: "GET" });
        if(!response.ok){
            throw new Error(`Http error. Response status: ${response.status}`);
        }
        const data = await response.json();

        renderItems(data);

    }
    catch (err) {
        console.log(`Error getting items. Error: ${err}`);
    }
}

async function search(){
    const searchInput = document.getElementById("search-input");
    const path = new URLSearchParams(location.search).get("path") || "";
    const response = await fetch(`/api/v1/items/search?path=${encodeURIComponent(path)}&query=${searchInput.value}`, { method: "GET" });

    if(!response.ok){
        throw new Error(`Http error. Response status: ${response.status}`);
    }
    const data = await response.json();

    renderItems(data);
}

function renderItems(data){
    const tbody = document.getElementById("item-list");
    const itemsShown = document.getElementById("items-shown");
    const totalSize = document.getElementById("total-size");

    tbody.innerHTML = "";
    for(const item of data.items){
        const isFolder = item.type === "Folder";
        const isFile = item.type === "File";
        const actionsCell = isFile
            ? `<a href="/api/v1/items/download?path=${encodeURIComponent(item.path)}" download>Download</a>`
            : "";

        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${item.name}</td>
            <td>${item.type}</td>
            <td>${actionsCell}</td>
        `;

        if (isFolder) {
            const nameCell = row.cells[0];
            nameCell.style.cursor = "pointer";
            nameCell.style.color = "blue";
            nameCell.addEventListener("click", function(event){
                navigateToPath(item.path);
            });
        }
        tbody.appendChild(row);
    }

    const totalItemCount = data.fileCount + data.folderCount;
    itemsShown.textContent = totalItemCount;
    totalSize.textContent = ((data.totalSize / (1024))).toFixed(1); // Conver bytes to KB 
}

function navigateToPath(path){
    history.pushState({ path }, "", `?path=${encodeURIComponent(path)}`);
    getItems(path);
}

function reloadBreadCrumbs(path){
    const breadcrumbElement = document.getElementById("breadcrumbs");
    breadcrumbElement.innerHTML = "";

    // adding a home link to naviage to root
    const home = document.createElement("a");
    home.textContent = "Home";
    home.href = "#";
    home.addEventListener("click", (e) => {
        e.preventDefault();
        navigateToPath("");
    });
    breadcrumbElement.appendChild(home);

    const folderList = path === "" ? [] : path.split("/");
    let cumulative = "";

    folderList.forEach((folder, index, folderList) => {
        cumulative = cumulative === "" ? folder : `${cumulative}/${folder}`;
        breadcrumbElement.append(" / ");

        const linkElement = document.createElement("a");
        linkElement.textContent = folder;
        linkElement.href = "#";
        const targetPath = cumulative;
        linkElement.addEventListener("click", function(event) {
            event.preventDefault();
            navigateToPath(targetPath);
        });
        breadcrumbElement.appendChild(linkElement);
        
    });
}

async function upload(){
    const uploadFileInputElement = document.getElementById("upload-file-input");
    const file = uploadFileInputElement.files[0];

    if(!file) {
        return;
    }

    const path = new URLSearchParams(location.search).get("path") || "";

    const formData = new FormData();
    formData.append("file", file);

    const response = await fetch(`/api/v1/items/upload?path=${encodeURIComponent(path)}`, { method: "POST", body: formData });

    if(!response.ok){
        console.error("Upload faliled");
        return;
    }

    getItems(path);
    uploadFileInputElement.value = "";
  
}

function clear(){
    const searchInputElement = document.getElementById("search-input");
    searchInputElement.value = "";

    const path = new URLSearchParams(location.search).get("path") || "";
    getItems(path);

}

function init(){

    const initialPath = new URLSearchParams(location.search).get("path") || "";
    getItems(initialPath);

    window.addEventListener("popstate", function(event){
        const path = new URLSearchParams(location.search).get("path") || "";
        getItems(path);
    } );

    const searchButton = document.getElementById("search-button");
    searchButton.addEventListener("click", search);

    const uploadButtonElement = document.getElementById("upload-button");
    uploadButtonElement.addEventListener("click", upload);

    const clearButtonElement = document.getElementById("clear-button");
    clearButtonElement.addEventListener("click", clear);

}

init();

