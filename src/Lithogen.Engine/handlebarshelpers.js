"use strict";

/*
From the AssembleIO docs. See also https://github.com/assemble/handlebars-helpers
for many examples of helpers.

Expression helpers
==================
are basically regular functions that take the name of the helper and the helper function as arguments.
Once an expression helper is registered, it can be called anywhere in your templates, then Handlebars takes the
expression's return value and writes it into the template.

In the template:

    {{phil}}

And the definition in JavaScript:

    module.exports = {
        "phil" : function() {
            return "hello PHIL";
        }


Block Helpers
=============
There are a few block helpers included by default with Handlebars, {{#each}}, {{#if}} and {{#unless}}. Custom block
helpers are registered the same way as expression helpers, but the difference is that Handlebars will pass the
contents of the block compiled into a function to the helper.

In the template:

    {{#blurb arg1 223 hello}}
       text to be output if the helper condition is true
    {{else}}
       text to be output if the helper condition is false
    {{/blurb}}

And the definition in JavaScript. Note we get an options object which contains the true and false templates:

    blurb: function (arg1, arg2, arg3, options) {
        if (something is true) {
            return options.fn(this);
        }
        return options.inverse(this);
    }
*/

console.log("Node shim working");

// Any function exported by this module gets registered as a helper.
module.exports = {
    "phil" : function() {
        return "hello PHIL";
    }

//    "price": require("./src/price.js"),
};
