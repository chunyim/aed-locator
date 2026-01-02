async function search() {
    const cityInput = document.getElementById('cityInput');
    const postalInput = document.getElementById('postalInput');
    const btn = document.getElementById('searchBtn');
    const btnText = document.getElementById('btnText');
    const tbody = document.querySelector('#aedTable tbody');
    const countText = document.getElementById('resultCount');

    // Clean up the postal code (remove spaces)
    const postal = postalInput.value.replace(/\s+/g, '');
    const city = cityInput.value;

    // 1. UI Loading State
    btn.classList.add('loading');
    btn.disabled = true;
    btnText.innerText = "Searching...";
    tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; padding:30px;">Connecting to Azure Database...</td></tr>';

    try {
        // 2. THE LIVE API URL
        const baseUrl = `https://aedlocator2026.azurewebsites.net/api/Aeds/search`;
        const url = `${baseUrl}?city=${encodeURIComponent(city)}&postalCode=${encodeURIComponent(postal)}`;

        const response = await fetch(url);

        if (!response.ok) throw new Error("Server responded with an error");

        const data = await response.json();

        tbody.innerHTML = '';

        // 3. Handle No Results
        if (data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; padding:30px;">No AEDs found for this area.</td></tr>';
            countText.innerText = "0 results found.";
        } else {
            countText.innerText = `Found ${data.length} life-saving device(s).`;

            // 4. Build Table Rows
            data.forEach(aed => {
                const fullAddress = `${aed.address}, ${aed.city}, ON`;
                const googleMapsUrl = `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(fullAddress)}`;

                const row = `<tr>
                    <td data-label="Site"><strong>${aed.site || 'N/A'}</strong></td>
                    <td data-label="Address">${aed.address || 'N/A'}</td>
                    <td data-label="Postal">${aed.postalCode || 'N/A'}</td>
                    <td data-label="Placement">${aed.placement || 'Not specified'}</td>
                    <td><a href="${googleMapsUrl}" target="_blank" class="map-btn">Directions</a></td>
                </tr>`;
                tbody.innerHTML += row;
            });
        }
    } catch (error) {
        // 5. Error Handling (CORS or Connection issues)
        tbody.innerHTML = `<tr><td colspan="5" style="text-align:center; color:red; padding:20px;">
            Unable to connect to the AED database. <br>
            <small>Check if CORS is enabled in Azure Settings.</small>
        </td></tr>`;
        console.error('Fetch error:', error);
    } finally {
        // 6. Reset Button State
        btn.classList.remove('loading');
        btn.disabled = false;
        btnText.innerText = "Search AEDs";
    }
}

// Reset Function
function resetSearch() {
    document.getElementById('cityInput').value = '';
    document.getElementById('postalInput').value = '';
    document.querySelector('#aedTable tbody').innerHTML = '';
    document.getElementById('resultCount').innerText = '';
}

// CSV Export Logic
function downloadCSV() {
    const rows = document.querySelectorAll("#aedTable tr");
    if (rows.length <= 1) return alert("Search for data first.");

    let csv = Array.from(rows).map(row =>
        Array.from(row.querySelectorAll("th, td")).slice(0, -1)
            .map(cell => `"${cell.innerText.replace(/"/g, '""')}"`).join(",")
    ).join("\n");

    const blob = new Blob([csv], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'AED_Locator_Results.csv';
    a.click();
}

// Allow "Enter" key to trigger search
document.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') search();
});