async function getItems(){
    try{
        console.log("Getting items...");
        const response = await fetch("/api/v1/items");
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
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${item.name}</td>
                <td>${item.type}</td>
                <td></td>
            `;
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

getItems();