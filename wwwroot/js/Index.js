function Init() {
    var http = new XMLHttpRequest();
    http.open("GET", getDashboardsUrl, true);
    http.responseType = 'json';
    http.setRequestHeader("Content-type", "application/json");
    http.onreadystatechange = function () {
        if (http.readyState == 4 && http.status == 200) {
            ListDashboards.call(this, typeof http.response == "object" ? http.response : JSON.parse(http.response));
        }
        else if (http.readyState == 4 && http.status == 404) {
            console.log("Server not found");
        }
        else if (http.readyState == 4) {
            console.log(http.statusText);
        }
    };

    http.send();
};

function ListDashboards(data) {
    if (typeof (data) != "undefined" && data != null) {
        renderDashboard(data[0].Id);
        data.forEach(function (element) {
            var divTag = document.createElement("div");
            divTag.innerHTML = element.Name;
            divTag.className = "dashboard-item";
            divTag.setAttribute("onclick", "renderDashboard('" + element.Id + "')");
            divTag.setAttribute("name", element.Name);
            divTag.setAttribute("itemid", element.Id);
            divTag.setAttribute("version", element.Version);
            divTag.setAttribute("ispublic", element.IsPublic);
            document.getElementById("panel").appendChild(divTag);
        });
    }
}

function getEmbedToken() {
    return fetch(tokenGenerationUrl, { // Backend application URL
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({})
    })
        .then(response => {
            if (!response.ok) throw new Error("Token fetch failed");
            return response.text();
        });
}

function renderDashboard(dashboardId) {
    this.getEmbedToken()
        .then(accessToken => {
            const dashboard = BoldBI.create({
                serverUrl: rootUrl + "/" + siteIdentifier,
                dashboardId: dashboardId,
                embedContainerId: "dashboard",
                embedToken: accessToken
            });

            dashboard.loadDashboard();
        })
        .catch(err => {
            console.error("Error rendering dashboard:", err);
        });
};