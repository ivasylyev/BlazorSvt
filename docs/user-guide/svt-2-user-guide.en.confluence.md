# SVT 2.0 User Guide

Format: Confluence Wiki Markup.  
Images: `images/` folder next to this file (replace with English screenshots when publishing).  
Source (RU): `svt-2-user-guide.ru.confluence.md`

---

h1. SVT 2.0 User Guide

{info:title=Who this guide is for}
This document is intended for logistics and tariff specialists.

No IT background is required.

Reference-book field meanings are the same as in the current SVT system.
{info}

----

h2. 1. What SVT 2.0 is and how it differs from the legacy system

*SVT 2.0* is the new version of the Tariff Management System. At this stage you can *view* the reference books that have already been migrated.

||Action||Where to do it||
|View migrated reference books (rates, average rates, parities, transport legs, locations-nodes)|*SVT 2.0*|
|Load and change data for these reference books|*Legacy* SVT|
|View reference books that are *not* migrated yet|*Legacy* SVT|

{warning:title=Important}
SVT 2.0 is read-only. You cannot create, edit, or delete records here yet.
{warning}

SVT 2.0 data is synchronized from the legacy system. After a change in legacy SVT, the update usually appears in SVT 2.0 *within about 2 minutes*.

----

h2. 2. How to open the system

Use one of the URLs below (corporate network access is required):

||Environment||URL||
|Test|[https://s001tst-as-svt.sibur.local/v2]|
|Prod|[https://s001as-svt.sibur.local/v2]|

# Open the link in the browser (Edge).
# If the browser prompts for credentials, sign in with your *Windows domain account* (the same one you use to log on to your PC).
# The SVT 2.0 home page opens.

----

h2. 3. Menu and language

At the top of the page you will find section buttons and the language selector.

!01-menu-header.png|width=800,alt="SVT 2.0 menu"!

On the right side of the header is the UI language (for example, *English (United States)*). When you change the language, button and column labels change. Column visibility settings are stored *separately for each language* (see section 7).

||Menu button||Page title (as shown in the system)||
|Home|Summary «Average rates by direction»|
|Transport Rates|Rates|
|Average Rates|Average Rate|
|Parities|Parity rates|
|Transport Legs|Transport Legs|
|Locations-Nodes|Locations-Nodes|

----

h2. 4. Home page

The home page shows the *«Average rates by direction»* summary (product: polyolefins, multi-month window).

!02-home-grid-chart.png|height=250,alt="Home: table and chart"!

* On the left — a table of directions and average rates by month.
* On the right — a bar chart for the *selected* row.
* To change the chart, click another row in the table.

Product and direction filters on this page are not configurable by the user — this is a fixed summary.

----

h2. 5. Working with a reference book (common rules)

Open the required section from the menu. A table of records opens.

*Common for all reference books:*

* Data is view-only.
* By default, *non-archive* (active) records are shown.
* To view archive records, enable the *Archive* column in Settings and filter by it (section 6.5).
* A filter row is under the column headers.
* You can sort: click a column header.
* Pagination is at the bottom (*«1 - 10 from … items»*). Use the arrows or page numbers.
* The arrow on the left of a row opens the *detail card* (all fields of the record).

Buttons to the right of the page title:

* *Reports* — Excel export (section 8).
* *Reset Filters* — clear all filters in the table.
* *Settings* — choose which columns to show (section 7).

h3. 5.1. Detail card

# Find the required row.
# Click the arrow on the left of the row.
# Field groups open (you can expand and collapse them). *Collapse all* / *Expand all* control all groups at once.

!08-details_average_rate.png|height=250,alt="Average Rate detail card"!

----

h2. 6. Filters (detailed)

Filters are located *under the column headers*. You can set several filters at once — only rows that match *all* conditions remain.

To clear all filters, click *Reset Filters*.

The filter types below are shown for the *Average Rate* reference book. Other books use the same filter types; column names may differ.

h3. 6.1. Drop-down list

Used for fields such as *Rate type*, *Transport kind*, *Transport type*, *Currency*.

# In the filter row under the column, open the list (often labeled *Select*).
# Choose a value (for example, *Tender*).
# The table refreshes.

!03-rate-type-filter.png|height=250,alt="Filter: rate type"!

h3. 6.2. Text search (nodes, names, product groups)

Used for *From*, *Proxy*, *To*, *Group*, *Product*, and similar text columns.

This is full-text search: the system finds records that contain the entered fragment.

# Click the filter field under the column.
# Enter part of the name (for example, *Kazan*, *port*, *China*).
# Wait for the table to refresh.

{tip:title=Tip}
Enter *at least 3 characters*. A shorter string will not work.
{tip}

You can combine several text filters (for example, From + Proxy + To).

!04-filter-nodefom-proxynode-nodeto.png|height=250,alt="Filter by nodes"!

h3. 6.3. Dates (start and end of the validity period)

Used for *Start*, *End*, and other date columns.

# Click the filter icon (funnel) on the date column — a list of operators opens.
# Choose an operator, for example:
** *Equals*
** *Greater Than* / *Greater Than Or Equals*
** *Less Than* / *Less Than Or Equals*
** *Clear* — remove the filter for this column
# Enter the date in the format shown in the field (typically *dd.mm.yyyy*) or pick it from the calendar.

!05-filter-start-end-choose-operation.png|height=250,alt="Date filter: operator selection"!

h3. 6.4. Numbers (rates, load, and similar)

Used for *Avg rate*, *Per ton*, *Load*, and other numeric columns.

# If needed, choose an operator next to the filter icon (*Equals*, *Greater Than*, *Less Than*, and so on).
# Enter a number (for example, *100*).
# The table shows matching rows. The footer shows how many records were found (for example, *«1 - 4 from 4 items»*).

!06-filter-totalcostton.png|height=250,alt="Numeric filter by average rate"!

h3. 6.5. How to view archive records

By default, the archive is hidden.

# Click *Settings*.
# Enable the *Archive* column and click *Ok*.
# In the *Archive* column filter, select the required value (archive / active).
# When finished, you can hide the column again or click *Reset Filters*.

----

h2. 7. Configuring visible columns

If there are too many columns or a required column is missing, adjust visibility.

# Click *Settings*.
# Turn column switches on or off (green means the column is visible).
# *Ok* — save and close.
# *Cancel* — close without saving.
# *Reset* — restore the *default* column set (if settings were changed incorrectly).

!07-settings.png|height=250,alt="Column settings dialog"!

{info:title=Where settings are stored}
Settings are stored in *your browser on this computer*, separately for *Russian* and *English*. On another computer or in another browser, the default column set is used again. Changing the language does not copy your settings from one language to the other.
{info}

{warning:title=Please note}
After *Ok* or *Reset* in column settings, the current table filters are usually cleared. Set the filters again if needed.
{warning}

----

h2. 8. Excel reports

Export respects the *current table filters*.

# Click *Reports*.
# Choose:
** *Short report* — both visible and hidden grid columns;
** *Full report* — an extended field set (as in the detail card).
# The Excel file is downloaded to your browser downloads folder.

!11-report-menu.png|height=250,alt="Reports menu"!

If there are many rows, a confirmation dialog appears: the system warns about the row count and possible wait time. Click *Yes* to continue or *No* to cancel.

!12-report-confirm.png|height=250,alt="Large report confirmation"!

While the report is being generated, a status message may appear — wait until it finishes and do not close the tab.

{warning:title=Please note}
Prefer short reports when possible. They are generated about ~10 times faster than full reports.

You can export short reports of up to 200 thousand records.

Avoid exporting full reports larger than 20 thousand records. Use filters to limit the number of rows.
{warning}

----

h2. 9. If something goes wrong

Try the following first:

* refresh the page (F5);
* click *Reset Filters*;
* in *Settings*, click *Reset* (default columns);
* open the same URL in another browser or in InPrivate / Incognito mode.

If the issue remains, submit a request (see below) and attach the materials described in this section.

h3. 9.1. Where to send the request

||Stage||Contact||
|Pilot industrial operation|email [vasilevivv@sibur.ru]|
|Production operation|ticket in *VKUS* (internal portal)|

In the request, include:

* environment (test or prod) and the page URL;
* what you did (step by step);
* what you expected and what happened;
* screenshots and the error text (as below).

h3. 9.2. An error message is shown on the screen

Usually a banner appears at the top or in the page area:

_«A program error occurred. Click for details.»_

Do the following:

# Take a *screenshot of the whole page* (*PrtSc* / *Print Screen*, or *Win + Shift + S* to capture a region).
# Briefly write the *reproduction steps*, for example:
** opened Average Rate;
** entered «Kazan» in the From filter;
** clicked the row arrow — an error appeared.
# *Click* the error banner — details open (a dark box with text).
# Take a *second screenshot* with the details expanded, *or* select the text with the mouse (*Message*, *Type*, *StackTrace*), copy it (*Ctrl + C*), and paste it into the email / ticket.

!09-error-details.png|height=250,alt="Expanded error details"!

h3. 9.3. No error banner, but the page is blank, frozen, or the reference book did not load

Sometimes there is no red banner, but the table does not appear, keeps loading, or the page «freezes». In that case, capture information from the *browser console*.

Step by step (Edge / Chrome):

# Do not close the tab with the problem.
# Press *F12* on the keyboard.
## If F12 does nothing, try *Ctrl + Shift + I*.
## On a laptop you may need *Fn + F12*.
# A developer panel opens on the right or at the bottom. This is normal: you need only one tab.
# At the top of the panel, find the *Console* tab (in a Russian UI it may be labeled *«Консоль»*). Click it.
# The console shows text lines (some may be red or yellow).
# Take a *screenshot* of the whole console window, *or* select the text with the mouse, copy it (*Ctrl + C*), and paste it into the request.
# Close the panel with the × button or press *F12* again.

!10-console-details.png|height=250,alt="Browser console (F12)"!

{tip:title=Do not interpret the text yourself}
Attaching a screenshot or the copied text is enough. You do not need to interpret the messages — support specialists will do that.
{tip}

----

h2. 10. Quick reference

||Task||What to do||
|Open a reference book|Button in the top menu|
|Find records|Filters under column headers|
|Clear search|*Reset Filters*|
|Show / hide columns|*Settings* → switches → *Ok*|
|Restore default columns|*Settings* → *Reset*|
|View all fields of a record|Arrow on the left of the row|
|Export to Excel|*Reports* → short or full|
|Change language|Language button on the right of the header|
|Data seems ~1 minute stale|Wait up to ~2 minutes after the change in legacy SVT|
