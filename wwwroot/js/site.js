// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Sayfa yüklendiğinde çalışacak fonksiyonlar
$(document).ready(function () {
    bildirimSayisiniGuncelle();
    bildirimleriYukle();
    setInterval(bildirimSayisiniGuncelle, 30000); // Her 30 saniyede bir güncelle
});

// Bildirim işlemleri
function bildirimSayisiniGuncelle() {
    $.get('/Bildirim/OkunmamisBildirimSayisi', function (sayi) {
        const bildirimSayisi = $('#bildirimSayisi');
        if (sayi > 0) {
            bildirimSayisi.text(sayi).show();
        } else {
            bildirimSayisi.text('0').hide();
        }
    });
}

function bildirimleriYukle() {
    $.get('/Bildirim/OkunmamisBildirimler', function (bildirimler) {
        const bildirimListesi = $('#bildirimListesi');
        bildirimListesi.empty();

        if (bildirimler.length === 0) {
            bildirimListesi.append('<div class="p-3 text-center text-white-50">Yeni bildirim yok</div>');
            return;
        }

        bildirimler.forEach(function (bildirim) {
            const bildirimHtml = `
                <div class="dropdown-item text-white-50 p-2 border-bottom border-secondary">
                    <div class="d-flex align-items-center">
                        <i class="bi ${getBildirimIkonu(bildirim.tip)} me-2"></i>
                        <div>
                            <div>${bildirim.mesaj}</div>
                            <small class="text-muted">${formatTarih(bildirim.olusturulmaTarihi)}</small>
                        </div>
                    </div>
                </div>`;
            bildirimListesi.append(bildirimHtml);
        });
    });
}

// Yardımcı fonksiyonlar
function getBildirimIkonu(tip) {
    switch (tip) {
        case 0: return 'bi-bell text-info';
        case 1: return 'bi-check-circle text-success';
        case 2: return 'bi-exclamation-circle text-warning';
        case 3: return 'bi-plus-circle text-primary';
        default: return 'bi-bell';
    }
}

function formatTarih(tarih) {
    const date = new Date(tarih);
    return date.toLocaleDateString('tr-TR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Form işlemleri
function formGonder(form, basariliMesaj) {
    $(form).submit(function (e) {
        e.preventDefault();
        yuklemeGoster();

        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (yanit) {
                yuklemeGizle();
                if (yanit.basarili) {
                    basariliToastGoster(basariliMesaj);
                    setTimeout(() => window.location.href = yanit.yonlendir, 1000);
                } else {
                    hataToastGoster(yanit.mesaj || 'Bir hata oluştu');
                }
            },
            error: function () {
                yuklemeGizle();
                hataToastGoster('İşlem sırasında bir hata oluştu');
            }
        });
    });
}

// Görev işlemleri
function gorevDurumunuGuncelle(gorevId, tamamlandi) {
    yuklemeGoster();
    $.post('/Gorev/DurumGuncelle', { id: gorevId, tamamlandi: tamamlandi })
        .done(function (yanit) {
            yuklemeGizle();
            if (yanit.basarili) {
                basariliToastGoster(tamamlandi ? 'Görev tamamlandı' : 'Görev yeniden açıldı');
                setTimeout(() => window.location.reload(), 1000);
            } else {
                hataToastGoster(yanit.mesaj || 'Durum güncellenirken bir hata oluştu');
            }
        })
        .fail(function () {
            yuklemeGizle();
            hataToastGoster('İşlem sırasında bir hata oluştu');
        });
}

function gorevSil(gorevId) {
    if (confirm('Bu görevi silmek istediğinize emin misiniz?')) {
        yuklemeGoster();
        $.post('/Gorev/Sil', { id: gorevId })
            .done(function (yanit) {
                yuklemeGizle();
                if (yanit.basarili) {
                    basariliToastGoster('Görev başarıyla silindi');
                    setTimeout(() => window.location.reload(), 1000);
                } else {
                    hataToastGoster(yanit.mesaj || 'Görev silinirken bir hata oluştu');
                }
            })
            .fail(function () {
                yuklemeGizle();
                hataToastGoster('İşlem sırasında bir hata oluştu');
            });
    }
}
