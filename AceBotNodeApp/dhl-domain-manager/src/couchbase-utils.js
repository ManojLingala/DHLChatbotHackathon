const couchbase = require('couchbase');
const bluebird = require('bluebird');

let bucket;
function getBucket() {
  if (!bucket) {
    const cluster = new couchbase.Cluster('http://localhost:8091');
    const rawBucket = cluster.openBucket('DHLData', 'dhl');
    rawBucket.operationTimeout = 10000;
    bucket = bluebird.promisifyAll(rawBucket);
  }
  return bucket;
}

function getRecord(key) {
  return getBucket().getAsync(key)
    .then(response => Promise.resolve(response.value))
    .catch(error => Promise.reject(error));
}

function getCouchbaseOptions() {
  const ttlDays = 30;
  const offset = ttlDays * 24 * 60 * 60;
  const ttl = Math.floor(new Date().getTime() / 1000) + offset;
  return {expiry: ttl};
}

function postRecord(key, values) {
  return getBucket().upsertAsync(key, values, getCouchbaseOptions())
    .then(response => Promise.resolve(response.value))
    .catch(error => Promise.reject(error));
}

module.exports = {getBucket, getRecord, postRecord};