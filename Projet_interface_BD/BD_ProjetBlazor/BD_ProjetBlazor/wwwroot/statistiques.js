window.AfficherGraphiques = (labels, argent, personnes) => {

    console.log("LABELS:", labels);
    console.log("ARGENT:", argent);
    console.log("PERSONNES:", personnes);

    const ctxA = document.getElementById("chartArgent");
    const ctxP = document.getElementById("chartPersonnes");

    // Graphique ARGENT ($) - Couleur bleue
    new Chart(ctxA, {
        type: "line",
        data: {
            labels: labels,
            datasets: [{
                label: "Total $ par mois",
                data: argent,
                borderColor: "rgba(54, 162, 235, 1)",
                backgroundColor: "rgba(54, 162, 235, 0.3)",
                borderWidth: 3,
                tension: 0.3
            }]
        },
        options: {
            responsive: true
        }
    });

    // Graphique PERSONNES - Couleur verte
    new Chart(ctxP, {
        type: "bar",
        data: {
            labels: labels,
            datasets: [{
                label: "Nombre de personnes",
                data: personnes,
                backgroundColor: "rgba(75, 192, 92, 0.6)",
                borderColor: "rgba(75, 192, 92, 1)",
                borderWidth: 2
            }]
        },
        options: {
            responsive: true
        }
    });
};
