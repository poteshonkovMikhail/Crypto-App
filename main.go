package main

import (
	"encoding/json"
	"log"
	"net/http"
	"strings"
	"sync"
	"time"

	"github.com/markcheno/go-quote"
)

type Cryptocurrency struct {
	Name  string  `json:"name"`
	Price float64 `json:"price"`
}

var (
	cryptoData = make(map[string]float64)
	lock       = sync.RWMutex{}
)

func main() {
	go fetchCryptoPrices()

	http.HandleFunc("/api/cryptocurrencies", getCryptoPrices)
	log.Fatal(http.ListenAndServe(":8080", nil))
}

func fetchCryptoPrices() {
	for {
		lock.Lock()
		updateCryptoPrice("BTC-USDT", "BTC-USDT (Bitcoin)")
		updateCryptoPrice("ETH-USDT", "ETH-USDT (Ethereum)")
		updateCryptoPrice("AVAX-USDT", "AVAX-USDT (Avalanche)")
		updateCryptoPrice("SOL-USDT", "SOL-USDT (Solana)")
		updateCryptoPrice("ADA-USDT", "ADA-USDT (Cardano)")
		lock.Unlock()
		time.Sleep(30 * time.Second)
	}
}

func updateCryptoPrice(symbol string, name string) {
	todayDate := time.Now().AddDate(0, 0, -1)
	tomorrowDate := time.Now().AddDate(0, 0, 0)
	quote, err := quote.NewQuoteFromCoinbase(symbol, todayDate.String()[:strings.Index(todayDate.String(), " ")], tomorrowDate.String()[:strings.Index(tomorrowDate.String(), " ")], quote.Min1)
	if err != nil {
		log.Println("Error fetching price:", err)
		return
	}

	price := quote.Close[len(quote.Close)-1]
	cryptoData[name] = price
}

func getCryptoPrices(w http.ResponseWriter, r *http.Request) {
	lock.RLock()
	defer lock.RUnlock()
	cryptos := []Cryptocurrency{}
	for name, price := range cryptoData {
		cryptos = append(cryptos, Cryptocurrency{Name: name, Price: price})
	}
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(cryptos)
}
