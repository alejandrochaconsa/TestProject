async function getItems(path){
    try{
        reloadBreadCrumbs(path);
        
        const response = await fetch(`/api/v1/items?path=${encodeURIComponent(path)}`);
        if(!response.ok){
            throw new Error(`Http error. Response status: ${response.status}`);
        }
        const data = await response.json();
        console.log(data);

        const tbody = document.getElementById("item-list");
        const itemsShown = document.getElementById("items-shown");
        const totalSize = document.getElementById("total-size");

        tbody.innerHTML = "";
        for(const item of data.items){
            const isFolder = item.type === "Folder";
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${item.name}</td>
                <td>${item.type}</td>
                <td></td>
            `;

            if (isFolder) {
                row.style.cursor = "pointer";
                row.style.color = "blue";
                row.addEventListener("click", function(event){
                    navigateToPath(item.path);
                });
            }

            tbody.appendChild(row);
        }

        const totalItemCount = data.fileCount + data.folderCount;
        itemsShown.textContent = totalItemCount;
        totalSize.textContent = ((data.totalSize / (1024))).toFixed(1); // Conver bytes to KB 

    }
    catch (err) {
        console.log(`Error getting items. Error: ${err}`);
    }
}

function navigateToPath(path){
    history.pushState({ path }, "", `?path=${encodeURIComponent(path)}`);
    getItems(path);
}

window.addEventListener("popstate", function(event){
    const path = new URLSearchParams(location.search).get("path") || "";
    getItems(path);
} );

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
        linkElement.addEventListener("click", (e) => {
            e.preventDefault();
            navigateToPath(targetPath);
        });
        breadcrumbElement.appendChild(linkElement);
        
    });
}

const initialPath = new URLSearchParams(location.search).get("path") || "";
getItems(initialPath);