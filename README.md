# Üzembehelyezési útmutató

## Adatbázis

Az adatbázis létrehozásához és adatokkal való feltöltéséhez az SQL parancsok megtalálhatóak a `Fogadások.txt` fájlban.

> A `Bets` tábla *Status* cellája eltér az eredetitől, *INT* típusú, így a megfelelő működéshez elengedhetetlen, hogy a fájlban lévő parancsokat használjuk.

## Adatmodell, design tervek, szerepkörök leírása

Az elkészült adatmodell és a szerepkörök leírása a `Fogadások.txt` fájlban (főkönyvtár), a design terveket az erre létrehozott `design` (főkönyvtár) mappában találhatóak meg.

## Szerepkörök

- Fogadó bejelentkezés: felhasz: laci; jelszó: Asd123 (természetesen új fogadó fiókot is lehet regisztrálni)
- Szervező bejelentkezés: felhasz: Strof; jelszó: Asd123
- Admin bejelentkezés: felhasz: admin; jelszó: Admin123

## Üzembehelyezés

Az megfelelő adatbázis létrehozása után a programot csak indítani kell és működőképes lesz.

## Általános tudnivaló

A program automatikusan zárja le a fogadásokat, adminnak nincs rá lehetősége. Egy fogadás akkor záródik le, amikor az esemény időpontja eléri az aznapi dátumot, ekkor véletlenszerűen a program eldönti, hogy nyert vagy vesztett-e a felhasználó. 
