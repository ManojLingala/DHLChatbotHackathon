const couchbaseUtils = require('./couchbase-utils');

const generateOrder = (parcelId, details) => {
    return couchbaseUtils.postRecord(parcelId, details)
        .then(parcelInfo => {
            console.log(`Order is generated for ${parcelId}}`);
            return Promise.resolve(parcelInfo);
        })
        .catch(error => {
            console.log(`Error ${error} while creating order for Id ${parcelId}`);
            return Promise.reject(error);
        })
};

module.exports = {generateOrder};