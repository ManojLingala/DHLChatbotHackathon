const couchbaseUtils = require('./couchbase-utils');

const fetchDetails = (customerId) => {
  return couchbaseUtils.getRecord(customerId)
    .then(customerInfo => {
      console.log(`Parcel Info for Id ${customerId} is ${JSON.stringify(customerInfo)}`);
      return Promise.resolve(customerInfo);
    })
    .catch(error => {
      console.log(`Error ${error} while fetching Parcel Info for Id ${customerId}`);
      return Promise.reject(error);
    })
};

const fetchServiceLocations = (city) => {
    return couchbaseUtils.getRecord(`global-service-locations`)
        .then(serviceLocations => {
            console.log(`Service locations in city ${city} is ${JSON.stringify(serviceLocations.australia.melbourne)}`);
            return Promise.resolve(serviceLocations.australia.melbourne);
        })
        .catch(error => {
            console.log(`Error ${error} while fetching Service locations Info for city ${city}`);
            return Promise.reject(error);
        })
};

module.exports = {fetchDetails, fetchServiceLocations};